using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FPECS.ISTK.Service.Infrastructure.Responses;

public class CommonActionResult : IActionResult
{
    public object? Data { get; set; }
    public int StatusCode { get; set; } = (int)HttpStatusCode.OK;

    public Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new CommonObjectResult<object>(Data, StatusCode);

        if (Data is not ResponseModel<object>)
        {
            objectResult.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        return objectResult.ExecuteResultAsync(context);
    }
}

public class CommonObjectResult<TValue> : ObjectResult where TValue : class
{
    public CommonObjectResult(TValue? value, int status) : base(value)
    {
        Value = TypedValue = value;
        StatusCode = status;
        DeclaredType = typeof(TValue);
    }

    public TValue? TypedValue { get; }

    public static implicit operator CommonObjectResult<TValue>(TValue value) => new(value, (int)HttpStatusCode.OK);
}
