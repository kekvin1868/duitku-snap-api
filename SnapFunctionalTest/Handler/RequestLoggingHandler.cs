using Serilog;

namespace SnapFunctionalTest.Handler;

public class RequestLoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}