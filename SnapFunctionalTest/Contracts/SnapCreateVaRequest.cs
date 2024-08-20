namespace SnapFunctionalTest.Contracts;

public class SnapCreateVaRequest
{
    public string? PartnerServiceId { get; set; }
    public string? CustomerNo { get; set; }
    public string? VirtualAccountNo { get; set; }
    public string? VirtualAccountName { get; set; }
    public string? TrxId { get; set; }
    public string? ExpiredDate { get; set; } 
    public string? VirtualAccountTrxType { get; set; }
    public AmountModel? TotalAmount { get; set; }
}

public class AmountModel
{
    public string? Currency { get; set; } = "IDR"; // Default value is "IDR"
    public string? Value { get; set; }
}