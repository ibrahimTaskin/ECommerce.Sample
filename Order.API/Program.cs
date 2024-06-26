using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb"));
});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<PaymentCompletedEventConsumer>();
    config.AddConsumer<PaymentFailedEventConsumer>();
    config.AddConsumer<StockNotReservedEventConsumer>();
    config.UsingRabbitMq((context, _config) =>
    {
        _config.Host(builder.Configuration["RabbitMq"]);
        _config.ReceiveEndpoint(RabbitMqSettings.OrderStokReservedEventQueue,
               e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        _config.ReceiveEndpoint(RabbitMqSettings.PaymentFailedEventQueue,
             e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
        _config.ReceiveEndpoint(RabbitMqSettings.OrderStokNotReservedEventQueue,
              e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
    });
});

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
