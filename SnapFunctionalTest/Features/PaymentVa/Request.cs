using FastEndpoints;

namespace SnapFunctionalTest.Features.PaymentVa;

// public class Request : IPlainTextRequest
// {
//     public string Content { get; set; }
// }

public record Request(
    string PartnerServiceId,
    string CustomerNo,
    string VirtualAccountNo,
    string PaymentRequestId,
    string TrxId,
    AmountModel PaidAmount,
    AdditionalInfo AdditionalInfo);
    
    public record AmountModel(string Currency, string Value);
    public record AdditionalInfo(string Reference, string PaymentCode);