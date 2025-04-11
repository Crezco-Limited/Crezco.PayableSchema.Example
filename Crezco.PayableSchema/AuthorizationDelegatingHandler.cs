using System.Net.Http.Headers;

namespace Crezco.PayableSchema;

public class VersionDelegatingHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("Crezco-Version", "2024-06-30");
        
        return base.SendAsync(request, cancellationToken);
    }
}
public class AuthorizationDelegatingHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("CrezcoApiKey")
                     ?? throw new InvalidOperationException("CrezcoApiKey is not set in the environment variables");

        request.Headers.Authorization
            = new AuthenticationHeaderValue("Bearer", apiKey);
    
        return base.SendAsync(request, cancellationToken);
    }
}