using System.Globalization;

namespace SnapFunctionalTest.Contracts;

public static class SnapVaRequestFactory
{
    public static SnapCreateVaRequest CreateRequest(string partnerServiceId, string customerNo, string virtualAccountNo, string virtualAccountName, string trxId, decimal totalAmount)
    {
        var request = new SnapCreateVaRequest
        {
            PartnerServiceId = partnerServiceId,
            CustomerNo = customerNo,
            VirtualAccountNo = virtualAccountNo,
            VirtualAccountName = virtualAccountName,
            VirtualAccountTrxType = "O",
            TrxId = trxId,
            ExpiredDate = "2024-08-20T23:27:43+0700",
            TotalAmount = new AmountModel
            {
                Currency = "IDR",
                Value = totalAmount.ToString("F2", new CultureInfo("en-US"))
            }
        };

        if (totalAmount == 0)
        {
            request.VirtualAccountTrxType = "O";
        }

        return request;
    }
    
    public static SnapInquiryVaRequest CreateInquiryRequest(string partnerServiceId, string customerNo, string virtualAccountNo, string trxId)
    {
        return new SnapInquiryVaRequest
        {
            PartnerServiceId = partnerServiceId,
            CustomerNo = customerNo,
            VirtualAccountNo = virtualAccountNo,
            TrxId = trxId
        };
    }
    
    public static SnapDeleteVaRequest CreateDeleteRequest(string partnerServiceId, string customerNo, string virtualAccountNo, string trxId)
    {
        return new SnapDeleteVaRequest
        {
            PartnerServiceId = partnerServiceId,
            CustomerNo = customerNo,
            VirtualAccountNo = virtualAccountNo,
            TrxId = trxId
        };
    }
}