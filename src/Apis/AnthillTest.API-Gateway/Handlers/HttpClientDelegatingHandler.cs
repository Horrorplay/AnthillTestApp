namespace AnthillTest.API_Gateway.Handlers;

public class HttpClientDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor contextAccessor;

    public HttpClientDelegatingHandler(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorizationHeader = contextAccessor.HttpContext?.Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            if (request.Headers.Contains("Authorization"))
                request.Headers.Remove("Authorization");

            request.Headers.Add("Authorization", new List<string> { authorizationHeader! });
        }

        return base.SendAsync(request, cancellationToken);
    }
}
