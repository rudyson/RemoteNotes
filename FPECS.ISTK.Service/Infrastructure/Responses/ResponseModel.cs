namespace FPECS.ISTK.Service.Infrastructure.Responses;

public class ResponseModel<T> : ResponseModel
{
    public T? Data { get; set; }
}

public class ResponseModel
{
    public string? Message { get; set; }
    public IDictionary<string, string>? Errors { get; set; }
}

public class PaginatedResponseModel<TValue> : ResponseModel<TValue> where TValue : class
{
    public int Count { get; set; } = 0;
}