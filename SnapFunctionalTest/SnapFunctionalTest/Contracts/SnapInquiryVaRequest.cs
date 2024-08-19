namespace SnapFunctionalTest.Contracts;

public class SnapInquiryVaRequest
{
    public string PartnerServiceId { get; set; }
    public string CustomerNo { get; set; }
    public string VirtualAccountNo { get; set; }
    public string TrxId { get; set; }
}