using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Repository;
using Play.Catalog.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

//service settings
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//register mongo db service
builder.Services.AddSingleton(serviceProvider =>
{
    //mongo db settings
    var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
    //prepare mongo client
    var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
    //get mongo database with the same name as the current service
    return mongoClient.GetDatabase(serviceSettings.ServiceName);
});
//register repositories
builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();


//To Store guid type in mongo db as string
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));

//To Store DateTimeOffset type in mongo db as string
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
