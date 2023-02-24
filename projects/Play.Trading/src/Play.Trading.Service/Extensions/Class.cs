using MassTransit;
using Play.Common.MassTransit;
using Play.Trading.Service.StateMachines;

namespace Play.Trading.Service.Extensions
{
    public class Class
    {
        public static void AddMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.UsingPlayEconomyRabbitMQ();
                configure.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
                    .MongoDbRepository(r =>
                    {
                        var serviceSettings = Conf
                    });
            });
        }
    }
}
