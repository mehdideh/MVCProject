using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MVCProject.Data;
using MVCProject.Services;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
// Log.Logger = new LoggerConfiguration()
//     .WriteTo.File("Logs/Log.txt", rollingInterval: RollingInterval.Day)
//     .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});


//builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddScoped<Services>();
var app = builder.Build();

//app.UseMiddleware<MVCProject.Middlewares.ExceptionHandler>();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Swagger باید قبل از Authorization باشه
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

// Middlewares سفارشی
//app.UseMiddleware<MVCProject.Middlewares.ExceptionHandler>();
//app.UseMiddleware<MVCProject.Middlewares.LoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

// این خط‌های عجیب رو حذف کن
// ==app.MapStaticAssets();


app.Run();

