using System.Text.Json;
using FastEndpoints;
using SnapFunctionalTest.Filters;

namespace SnapFunctionalTest.Features.PaymentVa;

public class Endpoint : Endpoint<Request, Response>
{
    private static readonly JsonSerializerOptions JsonOption = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public override void Configure()
    {
        Post("/v1.0/transfer-va/payment");
        AllowAnonymous();
        Options(o => o.AddEndpointFilter<SnapSignatureFilter>());
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        const string virtualAccountNumber = "78287661234567891";
        var request = req; //JsonSerializer.Deserialize<RequestPaymentVa>(req.Content, JsonOption);
        
        if (request?.VirtualAccountNo != virtualAccountNumber)
        {
            await SendAsync(new Response("4042512", "Bill not found", null), cancellation: ct);
            return;
        }

        if (request.PaidAmount.Value == "1000")
        {
            await SendAsync(new Response("4042513", "Invalid Amount", null), cancellation: ct);
            return;
        }

        await SendAsync(new Response("2002500", "Success", new VaData(
            "7828766",
            "1234567891",
            "78287661234567891",
            "John Doe update",
            request.PaymentRequestId,
            request.TrxId,
            request.PaidAmount
            )), cancellation: ct);
    }
}