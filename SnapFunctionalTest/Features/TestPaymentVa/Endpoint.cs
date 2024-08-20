using FastEndpoints;
using SnapFunctionalTest.Contracts;
using SnapFunctionalTest.Interface;

namespace SnapFunctionalTest.Features.TestPaymentVa;

public class Endpoint : EndpointWithoutRequest
{
    private readonly ISnapTest _snapTest;

    public Endpoint(ISnapTest snapTest)
    {
        _snapTest = snapTest;
    }

    public override void Configure()
    {
        Get("/test-payment-va");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string partnerServiceId = "7828766";
        string customerId = "1234567891";
        string virtualAccountNo = $"{partnerServiceId}{customerId}";
        string trxId = "QP-Create-3"; 
        var request = new SnapPaymentVaRequest()
        {
            PartnerServiceId = partnerServiceId,
            CustomerNo = customerId,
            VirtualAccountNo = virtualAccountNo,
            TrxId = trxId,
            PaymentRequestId = "1234567891",
            PaidAmount = new AmountModel()
            {
                Value = "1000",
                Currency = "IDR"
            },
            AdditionalInfo = new AdditionalInfoModel("123123123123", "VA")
        };
        
        var response = await _snapTest.PaymentVa(
            request,
            "DS16006",
            Guid.NewGuid().ToString("N"));
        
        if (response.Content == null)
        {
            await SendAsync(response.Error.Content, cancellation: ct);
            return;
        }
        
        await SendAsync(response.Content, cancellation: ct);
    }
}