using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Play.Common.Identity;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Trading.Service.StateMachines;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
}).AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMongo()
    .AddJwtBearerAuthentication();

AddMassTransit(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


void AddMassTransit(IServiceCollection services)
{
    services.AddMassTransit(configure =>
    {
        configure.UsingPlayEconomyRabbitMQ();
        configure.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
            .MongoDbRepository(r =>
            {
                var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings))
                                        .Get<ServiceSettings>();
                var mongoSettings = builder.Configuration.GetSection(nameof(MongoDbSettings))
                                        .Get<MongoDbSettings>();

                r.Connection = mongoSettings.ConnectionString;
                r.DatabaseName = serviceSettings.ServiceName;
            });
    });

    services.AddMassTransitHostedService();
    services.AddGenericRequestClient();
}