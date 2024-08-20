namespace SnapFunctionalTest.Features.PaymentVa;

public record Response(string ResponseCode, string ResponseMessage, VaData? VirtualAccountData);

public record VaData(
    string PartnerServiceId, 
    string CustomerNo, 
    string VirtualAccountNo,
    string VirtualAccountName,
    string PaymentRequestId, 
    string TrxId, 
    AmountModel PaidAmount);

