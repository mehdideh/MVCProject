using System.Text.Json;
using AutoMapper.Internal;

namespace MVCProject.Middlewares;
public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;
    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch
        {
            Console.WriteLine("خطایی اتفاق افتاده");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new
            {
                message = "خطای سرور"
                
            };
            

            await context.Response.WriteAsJsonAsync(response);
        }

    }
}