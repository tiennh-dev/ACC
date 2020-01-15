using AutoMapper;
using Core.Cache.Redis.Implement;
using Core.Cache.Redis.Interface;
using Core.Common;
using Core.CQRS.Bus.Implements.RabitMq;
using Core.CQRS.Bus.Interfaces;
using Core.CQRS.Bus.Interfaces.RabitMq;
using Core.CQRS.Service.Implements;
using Core.CQRS.Service.Interfaces;
using Core.Elasticsearch;
using Core.Repository.Abstract;
using Core.Repository.Interface;
using Core.Resilience.Http;
using iChiba.OM.Cache.Interface;
using iChiba.OM.Cache.Interface.IFavoriteProductCache;
using iChiba.OM.Cache.Interface.IFavoriteSellerCache;
using iChiba.OM.Cache.Interface.YahooAuctions;
using iChiba.OM.Cache.Redis.Implement;
using iChiba.OM.Cache.Redis.Implement.YahooAuctions;
using iChiba.OM.DbContext;
using iChiba.OM.PrivateApi.AppService.Implement;
using iChiba.OM.PrivateApi.AppService.Implement.Adapter;
using iChiba.OM.PrivateApi.AppService.Implement.Configs;
using iChiba.OM.PrivateApi.AppService.Interface;
using iChiba.OM.PrivateApi.Configs;
using iChiba.OM.PrivateApi.Driver;
using iChiba.OM.PrivateApi.Extensions;
using iChiba.OM.Repository.Implement;
using iChiba.OM.Repository.Interface;
using iChiba.OM.Service.Implement;
using iChiba.OM.Service.Interface;
using Ichiba.Bank.Api.Driver;
using Ichiba.Cdn.Client.Implement;
using Ichiba.Cdn.Client.Implement.Configs;
using Ichiba.Cdn.Client.Interface;
using Ichiba.Partner.YahooAuction;
using Ichiba.Partner.YahooAuction.Config;
using iChibaShopping.Cms.PublicApi.Configs;
using iChibaShopping.Core.AppService.Implement;
using iChibaShopping.Core.AppService.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using tomware.Microwf.Engine;

namespace iChiba.OM.PrivateApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            LoadConfig();
        }

        private void LoadConfig()
        {
            ConfigurationRoot configurationRoot = (ConfigurationRoot)Configuration;
            IEnumerable<Microsoft.Extensions.Configuration.IConfigurationProvider> configurationProviders = configurationRoot
                .Providers
                .Where(p => p.GetType() == typeof(JsonConfigurationProvider)
                    && ((JsonConfigurationProvider)p).Source.Path.StartsWith("appsettings"));

            foreach (Microsoft.Extensions.Configuration.IConfigurationProvider item in configurationProviders)
            {
                ConfigSetting.Init<ConfigSettingDefine>(item);
                ConfigSetting.Init<RedisConnectionConfig>(item);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();

            Mapper.Initialize(cfg =>
            {
                cfg.ValidateInlineMaps = false;

                cfg.AddProfile<AdapterProfile>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            })
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "iChiba om private api",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
                c.CustomSchemaIds(x => x.FullName);
            })
            .AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region Add DI
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppConfig>>()?.Value);

            services.Configure<AuthorizeConfig>(Configuration.GetSection("Authorize"));
            services.Configure<FileUploadConfig>(Configuration.GetSection("FileStorage"));
            services.Configure<ElasticConnectionSetting>(Configuration.GetSection("ElasticConnectionSettings"));
            services.Configure<RabbitMqConnectionConfig>(Configuration.GetSection("RabbitMq:Connection"));
            services.Configure<ApiConfig>(Configuration.GetSection("BankApi"));

            // config yahoo auctions
            services.AddTransient<CategoryClient>();

            services.Configure<EndPointConfig>(Configuration.GetSection("YahooAuction:EndPoint"));
            services.Configure<List<RequestConfig>>(Configuration.GetSection("YahooAuction:Request"));

            services.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<EndPointConfig>>().Value;
            });

            services.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<ApiConfig>>().Value;
            });

            services.AddSingleton(sp =>
            {
                IOptions<List<RequestConfig>> requestConfigs = sp.GetRequiredService<IOptions<List<RequestConfig>>>();

                return new RequestConfigStoreSimple<RequestConfig>(requestConfigs.Value);
            });
            services.AddSingleton(sp =>
            {
                IOptions<List<Ichiba.Partner.YahooShopping.Config.RequestConfig>> requestConfigs = sp.GetRequiredService<IOptions<List<Ichiba.Partner.YahooShopping.Config.RequestConfig>>>();

                return new RequestConfigStoreSimple<Ichiba.Partner.YahooShopping.Config.RequestConfig>(requestConfigs.Value);
            });
            services.AddTransient(sp =>
            {
                return sp.GetRequiredService<RequestConfigStoreSimple<RequestConfig>>().GetConfig();
            });

            //config yahoo shopping
            services.Configure<Ichiba.Partner.YahooShopping.Config.EndPointConfig>(Configuration.GetSection("YahooShopping:EndPoint"));
            services.Configure<List<Ichiba.Partner.YahooShopping.Config.RequestConfig>>(Configuration.GetSection("YahooShopping:Request"));
            services.AddTransient<Ichiba.Partner.YahooShopping.CategoryClient>();

            services.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<Ichiba.Partner.YahooShopping.Config.EndPointConfig>>().Value;
            });

            services.AddTransient(sp =>
            {
                return sp.GetRequiredService<RequestConfigStoreSimple<Ichiba.Partner.YahooShopping.Config.RequestConfig>>().GetConfig();
            });

            //config rakuten
            services.Configure<Ichiba.Partner.Rakuten.Config.EndPointConfig>(Configuration.GetSection("Rakuten:EndPoint"));
            services.Configure<List<Ichiba.Partner.Rakuten.Config.RequestConfig>>(Configuration.GetSection("Rakuten:Request"));
            services.AddTransient<Ichiba.Partner.Rakuten.CategoryClient>();
            services.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<Ichiba.Partner.Rakuten.Config.EndPointConfig>>().Value;
            });
            services.AddSingleton(sp =>
            {
                IOptions<List<Ichiba.Partner.Rakuten.Config.RequestConfig>> requestConfigs = sp.GetRequiredService<IOptions<List<Ichiba.Partner.Rakuten.Config.RequestConfig>>>();

                return new RequestConfigStoreSimple<Ichiba.Partner.Rakuten.Config.RequestConfig>(requestConfigs.Value);
            });
            services.AddTransient(sp =>
            {
                return sp.GetRequiredService<RequestConfigStoreSimple<Ichiba.Partner.Rakuten.Config.RequestConfig>>().GetConfig();
            });

            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                ILogger<DefaultRabbitMqPersistentConnection> logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();
                RabbitMqConnectionConfig connectionConfig = sp.GetRequiredService<IOptions<RabbitMqConnectionConfig>>().Value;
                ConnectionFactory factory = new ConnectionFactory()
                {
                    HostName = connectionConfig.HostName,
                    UserName = connectionConfig.UserName,
                    Password = connectionConfig.Password
                };

                return new DefaultRabbitMqPersistentConnection(factory, logger);
            });

            services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(sp =>
            {
                ILogger<ResilientHttpClient> logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                int retryCount = 3;
                int exceptionsAllowedBeforeBreaking = 5;

                return new ResilientHttpClientFactory(logger, exceptionsAllowedBeforeBreaking, retryCount);
            });
            services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());

            services.AddDbContext<CustomerDBContext>(options => options.UseSqlServer(ConfigSettingDefine.DbiChibaCustomerConnectionString.GetConfig()));
            services.AddDbContext<WarehouseDBContext>(options => options.UseSqlServer(ConfigSettingDefine.DbWarehouseConnectionString.GetConfig()));

            services.AddScoped<IUnitOfWork, UnitOfWork<CustomerDBContext>>();
            services.AddScoped<IUnitOfWorkGeneric<WarehouseDBContext>, UnitOfWork<WarehouseDBContext>>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICurrentContext, CurrentContext>();
            services.AddTransient<IFileAppService, FileAppService>();
            services.AddTransient<IFileStorage, FileStorage>();
            services.AddSingleton<ElasticClientProvider<ElasticConnectionSetting>>();
            services.AddTransient<ElasticIndexer<ElasticConnectionSetting>>();
            services.AddTransient<IRedisStorage, RedisStorage>();

            services.AddTransient<iChibaShopping.Service.Interface.YahooAuctions.IProductService, iChibaShopping.Service.Implement.YahooAuctions.ProductService>();
            services.AddTransient<iChibaShopping.Index.Interface.YahooAuctions.IProductIndex, iChibaShopping.Index.Elasticsearch.Implement.YahooAuctions.ProductIndex>();

            services.AddTransient<IBankInfoService, BankInfoService>();
            services.AddTransient<IBankInfoAppService, BankInfoAppService>();
            services.AddTransient<IBankInfoRepository, BankInfoRepository>();

            services.AddTransient<IBankTransactionHistoryService, BankTransactionHistoryService>();
            services.AddTransient<IBankTransactionHistoryAppService, BankTransactionHistoryAppService>();
            services.AddTransient<IBankTransactionHistoryRepository, BankTransactionHistoryRepository>();

            services.AddTransient<ICustomerAppService, CustomerAppService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();

            services.AddTransient<ICustomerAddressAppService, CustomerAddressAppService>();
            services.AddTransient<ICustomerAddressRepository, CustomerAddressRepository>();
            services.AddTransient<ICustomerAddressService, CustomerAddressService>();

            services.AddTransient<IEmployessAppservice, EmployessAppService>();
            services.AddTransient<IEmployessRepository, EmployessRepository>();
            services.AddTransient<IEmployessCoreService, EmployessCoreService>();


            services.AddTransient<ICustomerProfileAppService, CustomerProfileAppService>();
            services.AddTransient<ICustomerProfileService, CustomerProfileService>();
            services.AddTransient<ICustomerProfileRepository, CustomerProfileRepository>();


            services.AddTransient<ISuccessfulBidAppService, SuccessfulBidAppService>();
            services.AddTransient<ISuccessfulBidService, SuccessfulBidService>();
            services.AddTransient<ISuccessfulBidRepository, SuccessfulBidRepository>();


            services.AddTransient<IBankClientInfoAppService, BankClientInfoAppService>();
            services.AddTransient<IBankClientInfoCache, BankClientInfoCache>();

            services.AddTransient<IDepositsAppService, DepositsAppService>();
            services.AddTransient<IDepositsRepository, DepositsRepository>();
            services.AddTransient<IDepositsService, DepositsService>();

            services.AddTransient<IFreezeAppService, FreezeAppService>();
            services.AddTransient<IFreezeRepository, FreezeRepository>();
            services.AddTransient<IFreezeService, FreezeService>();

            services.AddTransient<IWithDrawalAppService, WithDrawalAppService>();
            services.AddTransient<IWithDrawalRepository, WithDrawalRepository>();
            services.AddTransient<IWithDrawalService, WithDrawalService>();

            services.AddTransient<IWalletTransAppService, WalletTransAppService>();
            services.AddTransient<IWalletTransRepository, WalletTransRepository>();
            services.AddTransient<IWalletTransService, WalletTransService>();

            services.AddTransient<IOrderBuyForYouAppService, OrderBuyForYouAppService>();
            services.AddTransient<IOrderAppService, OrderAppService>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderService, OrderService>();

            services.AddTransient<IOrderdetailRepository, OrderdetailRepository>();
            services.AddTransient<IOrderdetailService, OrderdetailService>();

            services.AddTransient<IPaymentAppService, PaymentAppService>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IPaymentService, PaymentService>();

            services.AddTransient<IUserBidProductAppService, UserBidProductAppService>();
            services.AddTransient<IUserBidProductCache, UserBidProductCache>();

            services.AddTransient<IProductShortCache, ProductShortCache>();

            services.AddTransient<IProductBidClientCache, ProductBidClientCache>();
            services.AddTransient<IProductBidClientInfoAppService, ProductBidClientInfoAppService>();

            services.AddTransient<IBidLastTimeCache, BidLastTimeCache>();
            services.AddTransient<IBidLastTimeAppService, BidLastTimeAppService>();

            services.AddSingleton<IAuthorizeClient, AuthorizeClientImplement>();
            services.AddTransient<ApiClient>();

            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddTransient<IWalletService, WalletService>();
            services.AddTransient<IWalletAppService, WalletAppService>();

            services.AddTransient<IWarehouseRepository, WarehouseRepository>();
            services.AddTransient<IWarehouseService, WarehouseService>();
            services.AddTransient<IWarehouseAppService, WarehouseAppService>();

            services.AddTransient<IWarehouseEmpRepository, WarehouseEmpRepository>();
            services.AddTransient<IWarehouseEmpService, WarehouseEmpService>();
            services.AddTransient<IWarehouseEmpAppService, WarehouseEmpAppService>();

            services.AddTransient<ICustomerWalletRepository, CustomerWalletRepository>();
            services.AddTransient<ICustomerWalletService, CustomerWalletService>();

            services.AddTransient<ICustomerWalletRepository, CustomerWalletRepository>();
            services.AddTransient<ICustomerWalletAppService, CustomerWalletAppService>();

            services.AddTransient<ICustomerBankInfoRepository, CustomerBankingInfoRepository>();
            services.AddTransient<ICustomerBankingInfoService, CustomerBankingInfoService>();
            services.AddTransient<ICustomerBankingInfoAppService, CustomerBankingInfoAppservice>();

            services.AddTransient<ILevelRepository, LevelRepository>();
            services.AddTransient<ILevelAppService, LevelAppService>();
            services.AddTransient<ILevelService, LevelService>();

            services.AddTransient<IFavoriteProductAppService, FavoriteProductAppService>();
            services.AddTransient<IFavoriteProductCache, FavoriteProductCache>();

            services.AddTransient<IFavoriteSellerAppService, FavoriteSellerAppService>();
            services.AddTransient<IFavoriteSellerCache, FavoriteSellerCache>();

            services.AddTransient<IPriceQuoteLastTimeAppService, PriceQuoteLastTimeAppService>();
            services.AddTransient<IPriceQuoteLastTimeCache, PriceQuoteLastTimeCache>();

            services.AddTransient<IAspNetUserCache, AspNetUserCache>();

            services.AddTransient<ILocationsCache, LocationsCache>();
            services.AddTransient<ILocationsNodeCache, LocationsNodeCache>();
            services.AddTransient<ILocationsParentNodeCache, LocationsParentNodeCache>();

            services.AddTransient<IOrderMessageRepository, OrderMessageRepository>();
            services.AddTransient<IOrderMessageService, OrderMessageService>();

            services.AddTransient<IDepositsRespository, DepositsMessageRespository>();
            services.AddTransient<IDepositsMessageService, DepositsMessageService>();

            services.AddTransient<IPaymentMessageRepository, PaymentMessageRepository>();
            services.AddTransient<IPaymentMessageService, PaymentMessageService>();

            services.AddTransient<IWithdrawalMessageRepository, WithDrawalMessageRepository>();
            services.AddTransient<IWithDrawalMessageService, WithDrawalMessageService>();

            services.AddTransient<IPaymentAccountRepository, PaymentAccountRepository>();
            services.AddTransient<IPaymentAccountAppService, PaymentAccountAppService>();
            services.AddTransient<IPaymentAccountService, PaymentAccountService>();


            services.AddTransient<IBidExternalConfigRepository, BidExternalConfigRepository>();
            services.AddTransient<IBidExternalConfigAppService, BidExternalConfigAppService>();
            services.AddTransient<IBidExternalConfigService, BidExternalConfigService>();

            services.AddTransient<IDebtAppService, DebtAppService>();

            services.AddTransient<IOrderPackageAppService, OrderPackageAppService>();
            services.AddTransient<IOrderPackageRepository, OrderPackageRepository>();
            services.AddTransient<IOrderPackageService, OrderPackageService>();

            services.AddTransient<IOrderPackageProductAppService, OrderPackageProductAppService>();
            services.AddTransient<IOrderPackageProductRepository, OrderPackageProductRepository>();
            services.AddTransient<IOrderPackageProductService, OrderPackageProductService>();

            services.AddTransient<IOrderTransportAppService, OrderTransportAppService>();

            services.AddTransient<IOrderServiceAppService, OrderServiceAppService>();
            services.AddTransient<IOrderServiceRepository, OrderServiceRepository>();
            services.AddTransient<IOrderServiceService, OrderServiceService>();

            services.AddTransient<IOrderServiceMappingRepository, OrderServiceMappingRepository>();
            services.AddTransient<IOrderServiceMappingService, OrderServiceMappingService>();

            services.AddTransient<IProductTypeAppService, ProductTypeAppService>();
            services.AddTransient<IProductTypeRepository, ProductTypeRepository>();
            services.AddTransient<IProductTypeService, ProductTypeService>();

            services.AddTransient<IProductOriginAppService, ProductOriginAppService>();
            services.AddTransient<IProductOriginRepository, ProductOriginRepository>();
            services.AddTransient<IProductOriginService, ProductOriginService>();

            services.AddTransient<ICustomerGroupAppService, CustomerGroupAppService>();
            services.AddTransient<ICustomerGroupRepository,CustomerGroupRepository>();
            services.AddTransient<ICustomerGroupService, CustomerGroupService>();

            services.AddTransient<IPurchaseReportAppService, PurchaseReportAppService>();
            services.AddTransient<IPurchaseReportRepository, PurchaseReportRepository>();
            services.AddTransient<IPurchaseReportService, PurchaseReportService>();

            services.AddTransient<IPurchaseReportDataRepository, PurchaseReportDataRepository>();
            services.AddTransient<IPurchaseReportDataService, PurchaseReportDataService>();

            services.AddTransient<IProductFromUrlAppService, ProductFromUrlAppService>();

            #region Cms event

            services.Configure<CmsEventRabbitMqConfig>(Configuration.GetSection("RabbitMq:Event:Cms"));

            services.AddSingleton(sp =>
            {
                var rabbitMqConfig = sp.GetRequiredService<IOptions<CmsEventRabbitMqConfig>>().Value;

                return new CmsEventRabbitMqConfig()
                {
                    Environment = rabbitMqConfig.Environment,
                    BrokerName = rabbitMqConfig.BrokerName,
                    RoutingKey = rabbitMqConfig.RoutingKey,
                    QueueName = rabbitMqConfig.QueueName
                };
            });

            services.AddTransient<CmsEventAppService>();
            services.AddTransient<IEventSender<CmsEventRabbitMqConfig>, EventSender<CmsEventRabbitMqConfig>>();
            services.AddSingleton<IEventBus<CmsEventRabbitMqConfig>, EventBusRabbitMq<CmsEventRabbitMqConfig>>();

            #endregion Cms event

            #region CS Command

            services.Configure<CSCommandRabbitMqConfig>(Configuration.GetSection("RabbitMq:Command:CS"));

            services.AddSingleton(sp =>
            {
                var rabbitMqConfig = sp.GetRequiredService<IOptions<CSCommandRabbitMqConfig>>().Value;

                return new CSCommandRabbitMqConfig()
                {
                    Environment = rabbitMqConfig.Environment,
                    BrokerName = rabbitMqConfig.BrokerName,
                    RoutingKey = rabbitMqConfig.RoutingKey,
                    QueueName = rabbitMqConfig.QueueName,
                    InstanceName = rabbitMqConfig.InstanceName,
                    ReceiveCommandTimeout = rabbitMqConfig.ReceiveCommandTimeout
                };
            });

            services.AddTransient<CSCommandAppService>();
            services.AddTransient<ICommandSender<CSCommandRabbitMqConfig>, CommandSender<CSCommandRabbitMqConfig>>();
            services.AddSingleton<ICommandBus<CSCommandRabbitMqConfig>, CommandBusRabbitMq<CSCommandRabbitMqConfig>>();

            #endregion CS Command

            #region YABid Command

            services.Configure<YABidCommandRabbitMqConfig>(Configuration.GetSection("RabbitMq:Command:YABid"));

            services.AddSingleton(sp =>
            {
                var rabbitMqConfig = sp.GetRequiredService<IOptions<YABidCommandRabbitMqConfig>>().Value;

                return new YABidCommandRabbitMqConfig()
                {
                    Environment = rabbitMqConfig.Environment,
                    BrokerName = rabbitMqConfig.BrokerName,
                    RoutingKey = rabbitMqConfig.RoutingKey,
                    QueueName = rabbitMqConfig.QueueName,
                    InstanceName = rabbitMqConfig.InstanceName,
                    ReceiveCommandTimeout = rabbitMqConfig.ReceiveCommandTimeout
                };
            });

            services.AddTransient<YABidCommandAppService>();
            services.AddTransient<ICommandSender<YABidCommandRabbitMqConfig>, CommandSender<YABidCommandRabbitMqConfig>>();
            services.AddSingleton<ICommandBus<YABidCommandRabbitMqConfig>, CommandBusRabbitMq<YABidCommandRabbitMqConfig>>();

            #endregion YABid Command

            #region Workflow

            services.AddApiServices<CustomerDBContext>(this.Configuration);

            #endregion Workflow

            #region IS4

            services.Configure<Ichiba.IS4.Api.Driver.AccessConfig>(Configuration.GetSection("IS4:AccessConfig"));
            services.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<Ichiba.IS4.Api.Driver.AccessConfig>>().Value;
            });
            services.AddSingleton<Ichiba.IS4.Api.Driver.IAuthorizeClient, AuthorizeClientImplement2>();
            services.AddTransient<Ichiba.IS4.Api.Driver.AccessClient>();

            services.AddTransient<IAccessAppService, AccessAppService>();

            #endregion IS4

            #region Ichiba.Partner.Api.Driver

            services.Configure<Ichiba.Partner.Api.Driver.ProductFromUrlConfig>(Configuration.GetSection("PartnerApi:ProductFromUrlConfig"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<Ichiba.Partner.Api.Driver.ProductFromUrlConfig>>().Value);
            services.AddSingleton<Ichiba.Partner.Api.Driver.IAuthorizeClient, AuthorizeClientImplement>();
            services.AddSingleton<Ichiba.Partner.Api.Driver.ProductFromUrlClient>();

            #endregion Ichiba.Partner.Api.Driver


            services.Configure<Ichiba.PurchaseReport.Api.Driver.PurchaseReportUrlConfig>(Configuration.GetSection("PurchaseReportApi:PurchaseReportUrlConfig"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<Ichiba.PurchaseReport.Api.Driver.PurchaseReportUrlConfig>>().Value);
            services.AddSingleton<Ichiba.PurchaseReport.Api.Driver.IAuthorizeClient, AuthorizeClientImplementPurchaseReport>();
            services.AddSingleton<Ichiba.PurchaseReport.Api.Driver.PurchaseReportUrlClient>();

            #region Dependency

            services.AddSingleton<OrderChangeHandle>(sp =>
            {
                var csConnectionString = ConfigSettingDefine.DbiChibaCustomerConnectionString.GetConfig();
                var logger = sp.GetRequiredService<ILogger<OrderChangeHandle>>();

                return new OrderChangeHandle(logger, csConnectionString, sp);
            });
            services.AddSingleton<DependencyApp>();

            #endregion Dependency

            // config DI to API
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    IOptions<AuthorizeConfig> authorizeConfig = serviceProvider.GetRequiredService<IOptions<AuthorizeConfig>>();

                    options.Authority = authorizeConfig.Value.Issuer;
                    options.RequireHttpsMetadata = authorizeConfig.Value.RequireHttpsMetadata;
                    options.ApiName = authorizeConfig.Value.ApiName;
                });

            services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>(sp =>
            {
                const int retryCount = 3;
                const int exceptionsAllowedBeforeBreaking = 5;
                //var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();

                return new ResilientHttpClientFactory(null, exceptionsAllowedBeforeBreaking, retryCount);
            });
            services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());

            #endregion Add DI
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                OnShutdown(app.ApplicationServices);
            });

            loggerFactory.AddSerilog();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}]{NewLine}{Message:lj}{NewLine}{NewLine}{Exception}{NewLine}{NewLine}")
                .WriteTo.RollingFile(@"./logs/log-{Date}.txt", outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}]{NewLine}{Message:lj}{NewLine}{NewLine}{Exception}{NewLine}{NewLine}")
                .CreateLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseCors("CorsPolicy")
                .UseAuthentication()
                .UseMvc()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "iChiba cms private api");
                    c.DocumentTitle = "iChiba cms private api documents";
                });

            app.SubscribeMessageHandlers();

            AppStart(app.ApplicationServices);
        }

        private void AppStart(IServiceProvider provider)
        {
            provider.GetService<ICommandBus<CSCommandRabbitMqConfig>>().CreateConsumerResultChannel();
            provider.GetService<ICommandBus<YABidCommandRabbitMqConfig>>().CreateConsumerResultChannel();
            provider.GetService<DependencyApp>().Start();
        }

        private void OnShutdown(IServiceProvider serviceProvider)
        {
            try
            {
                serviceProvider.GetRequiredService<DependencyApp>().Stop();
            }
            catch
            {
            }
        }
    }
}
