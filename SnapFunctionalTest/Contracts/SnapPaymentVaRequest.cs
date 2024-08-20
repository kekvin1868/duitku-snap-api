namespace SnapFunctionalTest.Contracts;

public class SnapPaymentVaRequest : SnapCreateVaRequest
{
    public string? PaymentRequestId { get; set; }
    public AmountModel? PaidAmount { get; set; }
    public AdditionalInfoModel? AdditionalInfo { get; set; }
}

public record AdditionalInfoModel(string Reference, string PaymentCode);