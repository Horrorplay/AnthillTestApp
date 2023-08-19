using Polly;

namespace AnthillTest.AuthService.Handlers;

public class HttpClientHandlerRetry
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<HttpResponseMessage> SendRequestWithRetry()
    {
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        HttpResponseMessage response = await retryPolicy.ExecuteAsync(async () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost:5002"));
            return await _client.SendAsync(request);
        });
        response.EnsureSuccessStatusCode();
        return response;
    }
}
