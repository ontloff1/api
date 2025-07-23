using Microsoft.EntityFrameworkCore;
using FiasApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database and services
builder.Services.AddDbContext<FiasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FiasDb")));

builder.Services.AddScoped<FiasService>();
builder.Services.AddHttpClient("Insecure")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }); // ✅ Критически важно

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

Console.WriteLine("[START] FIAS API запущен");

app.Run();
