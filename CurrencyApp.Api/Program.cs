using CurrencyApp.Api.Middleware;
using CurrencyApp.Application.Configuration;
using CurrencyApp.Application.Factories;
using CurrencyApp.Application.Interfaces;
using CurrencyApp.Application.Services;
using CurrencyApp.Infrastructure.Providers;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));

// Add services to the container.
builder.Services.AddHttpClient<ICurrencyProvider, NbpCurrencyProvider>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<CurrencySettings>>().Value;
    client.BaseAddress = new Uri(settings.NbpApiBaseUrl);
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();

builder.Services.AddScoped<CurrencyService>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("stats-limit", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
