using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FiasApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFiasService, FiasService>();

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();
