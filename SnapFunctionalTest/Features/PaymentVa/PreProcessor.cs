using FastEndpoints;
using Microsoft.Extensions.Options;
using SnapFunctionalTest.Helpers;

namespace SnapFunctionalTest.Features.PaymentVa;

public class PreProcessor : IPreProcessor<Request>
{
    private readonly IOptions<SnapSettings> _options;

    public PreProcessor(IOptions<SnapSettings> options)
    {
        _options = options;
    }

    public async Task PreProcessAsync(IPreProcessorContext<Request> context, CancellationToken ct)
    {
        context.HttpContext.Request.EnableBuffering();
        var stream = context.HttpContext.Request.Body;
        stream.Seek(0, SeekOrigin.Begin);
        var streamReader = new StreamReader(stream);
        string requestBody = await streamReader.ReadToEndAsync(ct);
        
        var timestamp = context.HttpContext.Request.Headers["X-TIMESTAMP"].ToString();
        var signature = context.HttpContext.Request.Headers["X-SIGNATURE"].ToString();
        var partnerId = context.HttpContext.Request.Headers["X-PARTNER-ID"].ToString();
        var externalId = context.HttpContext.Request.Headers["X-EXTERNAL-ID"].ToString();
        
        string httpMethod = context.HttpContext.Request.Method;
        string path = context.HttpContext.Request.Path;
        
        string stringToSign = $"{httpMethod}:{path}:{CryptoHelper.Sha256(requestBody)}:{timestamp}";
        
        if (!CryptoHelper.VerifySHA256WithRSA(stringToSign, signature, _options.Value.PublicKey))
        {
            await context.HttpContext.Response.SendAsync(
                new Response("4012500", "Unauthorized Signature", null)
            , 401, cancellation: ct);
            return;
        }
    }
}