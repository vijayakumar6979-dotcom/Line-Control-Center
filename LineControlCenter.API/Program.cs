using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Services;
using LineControlCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Register DbContext
builder.Services.AddDbContext<ManufacturingDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// 2. Register Services (Dependency Injection)
builder.Services.AddScoped<IManufacturingDbContext, ManufacturingDbContext>();

builder.Services.AddScoped<IBkFctUphService, BkFctUphService>();
builder.Services.AddScoped<IBkTestTarRawDataService, BkTestTarRawDataService>();

// 3. Add Controllers
builder.Services.AddControllers();

// 4. Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
