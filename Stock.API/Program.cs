using MassTransit;
using Shared.Settings;
using Stock.API.Consumers;
using Stock.API.Extensions;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<OrderCreatedEventConsumer>();

    config.UsingRabbitMq((context, _congif) =>
    {
        _congif.Host(builder.Configuration["RabbitMq"]);
        _congif.ReceiveEndpoint(RabbitMqSettings.OrderCreatedEventQueue,
                e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));        
    });
});

builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
await DbDataInitializer.AddDataToMongo(builder.Services);

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
