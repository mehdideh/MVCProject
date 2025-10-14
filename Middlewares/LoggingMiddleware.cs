namespace MVCProject.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Url Request : {context.Request.Path} at {DateTime.Now}");

        await _next(context);

        Console.WriteLine($"Response StatusCode is : {context.Response.StatusCode}");

    }
     public string ReturnName { get; set; }
    
}