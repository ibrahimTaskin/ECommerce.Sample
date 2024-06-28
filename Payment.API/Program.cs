using MassTransit;
using Payment.API.Consumers;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging(configure => configure.AddConsole())
        .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<StockReservedEventConsumer>();

    config.UsingRabbitMq((context, _congif) =>
    {
        _congif.Host(builder.Configuration["RabbitMq"]);
        _congif.ReceiveEndpoint(RabbitMqSettings.OrderStokReservedEventQueue,
                e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
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
