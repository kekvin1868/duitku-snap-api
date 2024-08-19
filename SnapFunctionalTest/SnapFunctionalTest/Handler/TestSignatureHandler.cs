using Microsoft.Extensions.Options;
using SnapFunctionalTest.Helpers;

namespace SnapFunctionalTest.Handler;

public class TestSignatureHandler : DelegatingHandler
{
    private readonly IOptions<SnapSettings> _options;

    public TestSignatureHandler(IOptions<SnapSettings> options)
    {
        _options = options;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpMethod = request.Method.Method;
        var path = request.RequestUri!.PathAndQuery;
        var body = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : string.Empty;
        var timestamp = DateTime.Now.ToString("O");
        
        var stringToSign = $"{httpMethod}:{path}:{CryptoHelper.Sha256(body)}:{timestamp}";
        var signature = "";//CryptoHelper.SHA256WithRSA(stringToSign, _options.Value.PrivateKey);
        request.Headers.Add("X-TIMESTAMP", timestamp);
        request.Headers.Add("X-SIGNATURE", signature);
        
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}