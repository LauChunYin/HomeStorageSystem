using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace HomeStorage.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            // 业务逻辑错误返回 400 Bad Request
            BusinessException ex => (HttpStatusCode.BadRequest, ex.Message),

            // 2. 实体的 Guard Clause 抛出的参数异常 -> 400
            ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),

            // 数据未找到返回 404 Not Found
            KeyNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            
            // 4. 越权操作 -> 403
            UnauthorizedAccessException ex => (HttpStatusCode.Forbidden, "您无权操作该资源"),

            // 未知错误返回 500
            _ => (HttpStatusCode.InternalServerError, "服务器内部发生错误，请联系管理员")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}