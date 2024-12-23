using FPECS.ISTK.Service.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace FPECS.ISTK.Service.Infrastructure.FilterAttributes;

/// <summary>
/// Handles <see cref="FluentValidation.ValidationException"/> and returns Http response with 422 error code
/// </summary>
public class FluentValidationExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private const string ValidationFailedResponseMessage = "Validation failed";

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is FluentValidation.ValidationException validationException)
        {
            var result = new ResponseModel<object>
            {
                Message = ValidationFailedResponseMessage,
                Errors = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().ErrorMessage
                        )
            };
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            context.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;
            context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(result, _jsonSerializerOptions));
            context.ExceptionHandled = true;
        }

        base.OnException(context);
    }
}