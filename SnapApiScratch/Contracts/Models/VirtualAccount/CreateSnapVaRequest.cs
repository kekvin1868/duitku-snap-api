namespace SnapApiScratch.Contracts.Models.VirtualAccount
{
    public class CreateSnapVaRequest
    {
        public string? PartnerServiceId { get; set; }
        public string? CustomerNo { get; set; }
        public string? VirtualAccountNo { get; set; }
        public string? VirtualAccountName { get; set; }
        public string? TrxId { get; set; }
        public string? ExpiredDate { get; set; }
        public string? VirtualAccountTrxType { get; set; }

        public AmountModel TotalAmount { get; set; } = new AmountModel();
    }

    public class AmountModel
    {
        public string? Value { get; set; }
        public string? Currency { get; set; }
    }
}