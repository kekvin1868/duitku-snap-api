using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using SnapFunctionalTest.Helpers;
using SnapFunctionalTest.Interface;

namespace SnapFunctionalTest.Handler;

public class AuthSignatureHandler : DelegatingHandler
{
    private readonly ISnapTokenStore _tokenStore;
    private readonly IOptions<SnapSettings> _setting;

    public AuthSignatureHandler(ISnapTokenStore tokenStore, IOptions<SnapSettings> setting)
    {
        _tokenStore = tokenStore;
        _setting = setting;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetToken();
        var httpMethod = request.Method.Method;
        var path = request.RequestUri!.PathAndQuery;
        var body = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : string.Empty;
        var timestamp = DateTime.Now.ToString("O");
        
        var stringToSign = $"{httpMethod}:{path}:{token}:{CryptoHelper.Sha256(body)}:{timestamp}";
        var signature = CryptoHelper.HMACSHA512(stringToSign, _setting.Value.SecretKey);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-TIMESTAMP", timestamp);
        request.Headers.Add("X-SIGNATURE", signature);
        
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}