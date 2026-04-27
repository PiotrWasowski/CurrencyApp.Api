using CurrencyApp.Api.Middleware;
using CurrencyApp.Application.Configuration;
using CurrencyApp.Application.Interfaces;
using CurrencyApp.Application.Services;
using CurrencyApp.Infrastructure.Providers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CurrencySettings>(builder.Configuration.GetSection("CurrencySettings"));

// Add services to the container.
builder.Services.AddHttpClient<ICurrencyProvider, NbpCurrencyProvider>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<CurrencySettings>>().Value;
    client.BaseAddress = new Uri(settings.NbpApiBaseUrl);
});

builder.Services.AddScoped<CurrencyService>();

builder.Services.AddControllers();
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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
