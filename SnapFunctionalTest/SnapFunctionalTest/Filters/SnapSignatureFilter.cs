using FastEndpoints;
using Microsoft.Extensions.Options;
using SnapFunctionalTest.Contracts;
using SnapFunctionalTest.Features.PaymentVa;
using SnapFunctionalTest.Helpers;

namespace SnapFunctionalTest.Filters;

public class SnapSignatureFilter : IEndpointFilter
{
    private readonly IOptions<SnapSettings> _options;

    public SnapSignatureFilter(IOptions<SnapSettings> options)
    {
        _options = options;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        context.HttpContext.Request.EnableBuffering();
        var streamReader = new StreamReader(context.HttpContext.Request.Body);
        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        string requestBody = await streamReader.ReadToEndAsync();
        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        
        var timestamp = context.HttpContext.Request.Headers["X-TIMESTAMP"].ToString();
        var signature = context.HttpContext.Request.Headers["X-SIGNATURE"].ToString();
        
        string httpMethod = context.HttpContext.Request.Method;
        string path = context.HttpContext.Request.Path;
        
        string stringToSign = $"{httpMethod}:{path}:{CryptoHelper.Sha256(requestBody)}:{timestamp}";
        
        if (!CryptoHelper.VerifySHA256WithRSA(stringToSign, signature, _options.Value.PublicKey))
        {
            return Results.Json(new SnapGenericResponse()
            {
                ResponseCode = "4012500",
                ResponseMessage = "Unauthorized Signature"
            }, statusCode: 401);
        }
        
        return await next(context);
    }
}