using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTrannsitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                //consumers registration
                //consumers are the classes that are charges of consuming messages from the RabbitMQ messages
                //we gonna register consumers by define the assembly that should have all the consumers already defined, and that's going to be the entry assemby for wichever microservice is invoking this class
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    //service settings
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    configurator.Host(rabbitMQSettings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

                    //in case there is a trouble in executing consumers code
                    configurator.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(5));
                        //with that logic, anytime a message is not able to be consumed by a consumer, it'll be retried 3 times with a 5 seconds delay
                    });
                });
            });

            services.AddMassTransitHostedService();


            return services;
        }
    }
}
