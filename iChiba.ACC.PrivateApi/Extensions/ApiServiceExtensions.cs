using iChiba.ACC.PrivateApi.AppService.Implement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using tomware.Microwf.Core;
using tomware.Microwf.Engine;

namespace iChiba.ACC.PrivateApi.Extensions
{
    public static class ApiServiceExtensions
    {
        //public static IServiceCollection AddApiServices<TContext>(
        //  this IServiceCollection services,
        //  IConfiguration configuration
        //) where TContext : EngineDbContext
        //{
        //    var workflowConf = CreateWorkflowConfiguration(); // GetWorkflowConfiguration(services);
        //    IOptions<ProcessorConfiguration> processorConf = GetProcessorConfiguration(configuration, services);

        //    services
        //     .AddWorkflowEngineServices<TContext>(workflowConf)
        //     .AddJobQueueServices<TContext>(processorConf.Value)
        //     .AddTestUserWorkflowMappings(CreateSampleUserWorkflowMappings());

            //services.AddTransient<IUserContextService, IdentityUserContextService>();

        //    services.AddTransient<IWorkflowDefinition, OrderWorkflow>();
        //    services.AddTransient<IWorkflowDefinition, DepositWorkflow>();
        //    services.AddTransient<IWorkflowDefinition, PaymentWorkflow>();
        //    services.AddTransient<IWorkflowDefinition, WithdrawWorkflow>();

        //    services.AddTransient<OrderWorkflowAppService>();
        //    services.AddTransient<DepositWorkflowAppService>();
        //    services.AddTransient<PaymentWorkflowAppService>();
        //    services.AddTransient<WithdrawWorkflowAppService>();

        //    //services.AddScoped<IMigrationService, MigrationService>();

        //    return services;
        //}

        //private static WorkflowConfiguration CreateWorkflowConfiguration()
        //{
        //    return new WorkflowConfiguration
        //    {
        //        Types = new List<WorkflowType> {
        //            new WorkflowType {
        //                Type = "OrderWorkflow",
        //                Title = "Order",
        //                Description = "order workflow process.",
        //                Route = "order"
        //            },
        //            new WorkflowType {
        //                Type = "DEPOSIT_WORKFLOW",
        //                Title = "Deposit",
        //                Description = "deposit workflow process.",
        //                Route = "deposit"
        //            },
        //            new WorkflowType {
        //                Type = "PAYMENT_WORKFLOW",
        //                Title = "Payment",
        //                Description = "payment workflow process.",
        //                Route = "payment"
        //            },
        //            new WorkflowType {
        //                Type = "WITHDRAWAL_WORKFLOW",
        //                Title = "Withdraw",
        //                Description = "withdraw workflow process.",
        //                Route = "withdraw"
        //            }
        //        }
        //    };
        //}

        //private static List<UserWorkflowMapping> CreateSampleUserWorkflowMappings()
        //{
        //    return new List<UserWorkflowMapping> {
        //        new UserWorkflowMapping {
        //            UserName = "admin",
        //            WorkflowDefinitions = new List<string> {
        //                OrderWorkflow.TYPE
        //            }
        //        },
        //        new UserWorkflowMapping {
        //            UserName = "admin",
        //            WorkflowDefinitions = new List<string> {
        //                DepositWorkflow.TYPE
        //            }
        //        },
        //        new UserWorkflowMapping {
        //            UserName = "admin",
        //            WorkflowDefinitions = new List<string> {
        //                PaymentWorkflow.TYPE
        //            }
        //        },
        //        new UserWorkflowMapping {
        //            UserName = "admin",
        //            WorkflowDefinitions = new List<string> {
        //                WithdrawWorkflow.TYPE
        //            }
        //        }
        //    };
        //}

        private static IOptions<WorkflowConfiguration> GetWorkflowConfiguration(
          IConfiguration configuration,
          IServiceCollection services
        )
        {
            var workflows = configuration.GetSection("Workflows");
            services.Configure<WorkflowConfiguration>(workflows);

            return services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<WorkflowConfiguration>>();
        }

        private static IOptions<ProcessorConfiguration> GetProcessorConfiguration(
          IConfiguration configuration,
          IServiceCollection services
        )
        {
            var worker = configuration.GetSection("Worker");
            services.Configure<ProcessorConfiguration>(worker);

            return services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<ProcessorConfiguration>>();
        }
    }
}
