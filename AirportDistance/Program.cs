using AirportDistance.Contracts.Interfaces;
using AirportDistance.Contracts.Requests;
using AirportDistance.Contracts.Wrappers;
using AirportDistance.Domain.Services;
using AirportDistance.Domain.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddTransient<IDistanceService, DistanceService>();
builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddTransient<IAirportService, AirportService>();
builder.Services.AddTransient<ICalculatorService, CalculatorService>();
builder.Services.AddTransient<IValidator<DistanceRequest>, AirportCodeValidator>();

builder.Services.AddSwaggerGen();

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
