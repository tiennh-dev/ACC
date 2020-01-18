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
using iChiba.ACC.DbContext;
using iChiba.ACC.PrivateApi;
using iChiba.ACC.PrivateApi.AppService.Implement;
using iChiba.ACC.PrivateApi.AppService.Implement.Configs;
using iChiba.ACC.PrivateApi.AppService.Interface;
using iChiba.ACC.PrivateApi.Configs;
using iChiba.ACC.PrivateApi.Driver;
using iChiba.ACC.PrivateApi.Extensions;
using iChiba.ACC.Repository.Implement;
using iChiba.ACC.Repository.Interface;
using iChiba.ACC.Service.Implement;
using iChiba.ACC.Service.Interface;
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

namespace iChiba.ACC.PrivateApi
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

                //cfg.AddProfile<AdapterProfile>();
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
                    Title = "iChiba acc private api",
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

            services.AddDbContext<ACCDBContext>(options => options.UseSqlServer(ConfigSettingDefine.DbiChibaAccConnectionString.GetConfig()));
            services.AddScoped<IUnitOfWork, UnitOfWork<ACCDBContext>>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICurrentContext, CurrentContext>();
            services.AddTransient<IFileAppService, FileAppService>();
            services.AddTransient<IFileStorage, FileStorage>();
            services.AddSingleton<ElasticClientProvider<ElasticConnectionSetting>>();
            services.AddTransient<ElasticIndexer<ElasticConnectionSetting>>();
            services.AddTransient<IRedisStorage, RedisStorage>();

            services.AddTransient<iChibaShopping.Service.Interface.YahooAuctions.IProductService, iChibaShopping.Service.Implement.YahooAuctions.ProductService>();
            services.AddTransient<iChibaShopping.Index.Interface.YahooAuctions.IProductIndex, iChibaShopping.Index.Elasticsearch.Implement.YahooAuctions.ProductIndex>();

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAccountAppService, AccountAppService>();
            services.AddTransient<IAccountRepository, AccountRepository>();




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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "iChiba ACC private api");
                    c.DocumentTitle = "iChiba acc private api documents";
                });

            app.SubscribeMessageHandlers();

        }

    }
}
