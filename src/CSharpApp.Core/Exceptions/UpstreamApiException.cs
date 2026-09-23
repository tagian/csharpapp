namespace CSharpApp.Core.Exceptions;

public sealed class UpstreamApiException : Exception
{
    public int StatusCode { get; }
    public string? ResponseBody { get; }
    public string? ContentType { get; }

    public UpstreamApiException(
        int statusCode,
        string? responseBody,
        string? contentType = null)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        ContentType = contentType;
    }
}