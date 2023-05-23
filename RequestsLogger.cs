using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class RequestsLogger
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public RequestsLogger(RequestDelegate next, ILogger<RequestsLogger> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the request details
        string requestPath = context.Request.Path;
        string requestMethod = context.Request.Method;
        string queryString = context.Request.QueryString.Value;

        logger.LogInformation($"Incoming request: {requestMethod} {requestPath}{queryString}");
        logger.LogInformation($"Incoming request | #{requestNum} | resource: {resource name} | HTTP Verb {HTTP VERB in capital letter (GET, POST, etc)}");
        
        

        // Pass the request to the next middleware
        await next(context);
    }
}
