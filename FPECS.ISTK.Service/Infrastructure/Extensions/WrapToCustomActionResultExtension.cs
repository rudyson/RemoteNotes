using FPECS.ISTK.Service.Infrastructure.Responses;
using System.Net;
using System.Security;

namespace FPECS.ISTK.Service.Infrastructure.Extensions;

public static class WrapToCommonActionResultExtension
{
    public static CommonActionResult WrapToActionResult(this object? item)
    {
        var statusCode = item is null ? HttpStatusCode.NoContent : HttpStatusCode.OK;
        return new CommonActionResult()
        {
            StatusCode = (int)statusCode,
            Data = new ResponseModel<object>
            {
                Data = item
            }
        };
    }

    public static CommonActionResult WrapToActionResult(this bool item)
    {
        return new CommonActionResult()
        {
            Data = new ResponseModel<object>
            {
                Data = item
            },
            StatusCode = item ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest
        };
    }

    public static CommonActionResult WrapToPaginatedActionResult(this object? item, object data, int amount)
    {
        return new CommonActionResult()
        {
            Data = new PaginatedResponseModel<object>
            {
                Data = data,
                Count = amount
            },
            StatusCode = item != null ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NoContent
        };
    }

    public static CommonActionResult WrapToActionResult(this HttpStatusCode statusCode, string? message = null,
        string? errorMessage = null)
    {
        return new CommonActionResult()
        {
            StatusCode = (int)statusCode,
            Data = new ResponseModel<object>
            {
                Message = message ?? statusCode.ToString(),
                Errors = errorMessage is null ? null : new Dictionary<string, string>() { { "$", errorMessage } }
            }
        };
    }

    public static CommonActionResult WrapToActionResult(this Exception exception)
    {
        var statusCode = exception switch
        {
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            SecurityException => HttpStatusCode.Forbidden,
            ArgumentNullException or ArgumentOutOfRangeException or ArgumentException
                => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            NotImplementedException or NotSupportedException
                => HttpStatusCode.NotImplemented,
            ObjectDisposedException => HttpStatusCode.Gone,
            InvalidDataException => HttpStatusCode.UnprocessableEntity,
            TimeoutException => HttpStatusCode.RequestTimeout,
            InvalidOperationException or OperationCanceledException
                => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        return new CommonActionResult()
        {
            StatusCode = (int)statusCode,
            Data = new ResponseModel<object>
            {
                Message = nameof(exception),
                Errors = new Dictionary<string, string>()
                {
                    { nameof(exception.Message), exception.Message },
                    { nameof(exception.StackTrace), exception.StackTrace ?? "" },
                    { nameof(exception.Data), exception.Data.ToString() ?? "" }
                }
            }
        };
    }
}