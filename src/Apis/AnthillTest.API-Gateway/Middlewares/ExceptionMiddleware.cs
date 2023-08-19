using AnthillTest.API_Gateway.Models.ErrorModels;
using AnthillTest.API_Gateway.Models.LogModels;
using AnthillTest.API_Gateway.Services.ElasticSearch.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AnthillTest.API_Gateway.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IElasticSearchService _elasticSearchService;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private ElasticSearchLogOptions _logOptions;

    public ExceptionMiddleware(RequestDelegate next,
                           IConfiguration configuration,
                           IElasticSearchService elasticSearchService,
                           ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _elasticSearchService = elasticSearchService;

        _logOptions = _configuration.GetSection("ElasticSearchLogOptions").Get<ElasticSearchLogOptions>()!;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }

        catch (BadHttpRequestException e)
        {
            await HandleExceptionAsync(httpContext, e, EnumRisks.Normal, (int)HttpStatusCode.BadRequest);
        }

        catch (ValidationException e)
        {
            await HandleExceptionAsync(httpContext, e, EnumRisks.NotRisky, (int)HttpStatusCode.BadRequest);
        }

        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e, EnumRisks.Normal);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception e, EnumRisks risk = EnumRisks.Normal, int statusCode = (int)HttpStatusCode.InternalServerError)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        object message = string.Empty;
        string messagePrefix = "Exception";
        string logIndex = _logOptions.LogIndex;

        var logDetail = new LogDetail
        {
            MethodName = e?.TargetSite?.DeclaringType?.FullName ?? string.Empty,
            Explanation = $"{messagePrefix} : {message ?? string.Empty}",
            Risk = (byte)EnumRisks.Normal,
            LoggingTime = DateTime.UtcNow.ToString()
        };

        try
        {
            bool clientExists = await _elasticSearchService.IndexExistsAsync(logIndex);
            if (!clientExists)
            {
                var indexCreated = await _elasticSearchService.CreateIndexAsync<LogDetail>(logIndex);
                if (!indexCreated)
                    throw new Exception($"{logIndex} not created");
            }

            await _elasticSearchService.CreateOrUpdateAsync(logIndex, logDetail);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "{Ошибка} --- {Имя метода} --- {Пояснение} --- {Уровень риска} --- {Время возникновения}",
            messagePrefix, logDetail.MethodName, logDetail.Explanation, logDetail.Risk, logDetail.LoggingTime);
        }

        await httpContext.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = "Internal Server Error"
        }.ToString());
    }
}
