using SnapApiScratch.Contracts.Helpers;
using SnapApiScratch.Contracts.Helpers.HelperModels;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FastEndpoints;
using System.Globalization;

namespace SnapApiScratch.Endpoints.AllPaymentGateway
{
    public class GetAllPgRequests
    {
        private readonly GetBearerToken _bearerTokenProvider;

        public GetAllPgRequests(GetBearerToken getBearerToken)
        {
            _bearerTokenProvider = getBearerToken;
        }
        public StringContent CreateVaRequest()
        {
            var random = new Random();
            string formattedDateTime = DateTimeOffset.Now
                .ToOffset(TimeSpan.FromHours(7))
                .ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            string partnerSID = "7828766";
            string genCustomerNo = $"{random.Next(1, 1000000000)}";
            string genTransactionId = $"SS{random.Next(1, 10000)}";
            string name = "Kevin H";
            string randomPart = random.Next(0, (int)Math.Pow(10, 11)).ToString("D" + 11);

            var requestBody = new
            {
                originalPartnerReferenceNo = "SS54997828766",
                originalReferenceNo = "DS198902416OQSXIQ10BTW3X",
                serviceCode = "51"
            };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            return content;
        }

        public async Task<HttpRequestMessage> CreateRequestHeaders (HttpRequestMessage currentHttpReq)
        {
            HttpRequestMessage finalRequest = await _bearerTokenProvider.GetAllHeaders(currentHttpReq);
            
            return finalRequest;
        }
    }
}

// === BODY REQUESTS ===

// CREATE VA
// var requestBody = new
// {
//     partnerServiceId = partnerSID,
//     customerNo = genCustomerNo,
//     virtualAccountNo = partnerSID + genCustomerNo,
//     virtualAccountName = name,
//     trxId = genTransactionId,
//     expiredDate = "2025-08-23T12:57:24+07:00",
//     virtualAccountTrxType = "O",
//     totalAmount = new
//     {
//         value = "0.00",
//         currency = "IDR"
//     }
// };

// UPDATE VA
// var requestBody = new
// {
//     partnerServiceId = partnerSID,
//     customerNo = genCustomerNo,
//     virtualAccountNo = partnerSID + genCustomerNo,
//     virtualAccountName = name,
//     trxId = "SS-466",
//     expiredDate = "2025-08-23T12:57:24+07:00",
//     virtualAccountTrxType = "O",
//     totalAmount = new
//     {
//         value = "0.00",
//         currency = "IDR"
//     },
//     additionalInfo = new
//     {
//         minAmount = "10000.00",
//         maxAmount = "100000.00"
//     }
// };

// INQUIRY VA
// var requestBody = new
// {
//     partnerServiceId = partnerSID,
//     customerNo = "884879342",
//     virtualAccountNo = "7828766884879342",
//     trxId = "SS-137"
// };

// DELETE VA
// var requestBody = new
// {
//     partnerServiceId = partnerSID,
//     customerNo = "884879342",
//     virtualAccountNo = "7828766884879342",
//     trxId = "SS-137"
// };

// INQUIRY STATUS VA
// var requestBody = new
// {
//     partnerServiceId = partnerSID,
//     customerNo = "780152211",
//     virtualAccountNo = "7828766780152211",
//     inquiryRequestId = "SS-466"
// };

// ACCOUNT BINDING
// var requestBody = new
// {
//     phoneNo = "0084444444",
//     merchantId = "DS19890",
//     additionalInfo = new
//     {
//         customerUniqueId = genTransactionId + "123"
//     },
//     redirectUrl = "google.com"
// };

// DEBIT PAYMENT REDIRECT
// var requestBody = new
// {
//     partnerReferenceNo = genTransactionId,
//     chargeToken = "SA",
//     additionalInfo = new
//     {
//         returnUrl = "google.com"
//     },
//     amount = new
//     {
//         value = "10000.00",
//         currency = "IDR"
//     }
// };

// DEBIT PAYMENT STATUS
// var requestBody = new
// {
//     originalPartnerReferenceNo = "SS6118",
//     originalReferenceNo = "DS1989024A0LPTKQZPEBIX0W",
//     serviceCode = "55"
// };

// DEBIT PAYMENT QRIS
// var requestBody = new
// {
//     partnerReferenceNo = genTransactionId + partnerSID,
//     validityPeriod = "2025-09-16T13:00:00+07:00",
//     additionalInfo = new
//     {
//         returnUrl = "google.com"
//     },
//     amount = new
//     {
//         value = "1.00",
//         currency = "IDR"
//     }
// };

// EXISTING QRIS PAYMENT
// API Response: {"partnerReferenceNo":"SS54997828766",
//"referenceNo":"DS198902416OQSXIQ10BTW3X",
//"redirectUrl":"https://sandbox.duitku.com/TopUp/v2/TopUpQrisPaymentPage.aspx?reference=SP24BCXHV5D85WQ0RJ8",
//"responseCode":"2004700","responseMessage":"SUCCESS",
//"qrContent":"00020101021226610016ID.CO.SHOPEE.WWW01189360091800202688450208202688450303UKE52047988530336054041.005802ID5906Duitku6015KAB. BANGKA BAR61053331562230519SP24BCXHV5D85WQ0RJ8630419E6"}

// EXISTING DEBIT PAYMENT
// API Response: {"responseCode":"2005400","responseMessage":"SUCCESS",
//"referenceNo":"DS1989024A0LPTKQZPEBIX0W","partnerReferenceNo":"SS6118",
//"webRedirectUrl":"https://id.uat.shp.ee/sppay_checkout_id?type=start&mid=10207279&target_app=shopee&medium_index=dFhkbmR1bTBIamhW8XPJak_0w8_oocc2fAPiogF0ZLZLNOxpxh3CnuTKRKHyNm8&order_key=X2Jm1F8WkW0fWZlDmZeTGHJmeeupGTS9keKEjX3xYnZRm7UY4wgDfvHjVWDIB4Bhy3Mj6hGXLGMElA&order_sn=113501475193763648&return_url=aHR0cDovL2FtZWxpYTo3Nzc3L2Zyb250ZW5kL1RvcFVwL3YyL1RvcFVwRHVpdGt1QXBwTGlua1Nob3BlZVBheS5hc3B4P2Ftb3VudD0xMDAwMDAwJmNsaWVudF9pZD1EdWl0a3UrVUFUJnJlZmVyZW5jZT1TQTI0MTZaWkNJTVBOVzNSUUpHJnJlZmVyZW5jZV9pZD1TQTI0MTZaWkNJTVBOVzNSUUpHJnJlc3VsdF9jb2RlPTIwMyZzaWduYXR1cmU9ZV9zRXUzbW5IQ3RPSU5ZOVQwRHEwWHFBb0dzMGUzZTdHUHdCb0wyaHp2USUzRA%3D%3D&source=web&token=dFhkbmR1bTBIamhW8XPJak_0w8_oocc2fAPiogF0ZLZLNOxpxh3CnuTKRKHyNm8"
//}

// EXISTING VA
// API Response: {"virtualAccountData":{"additionalInfo":{"minAmount":"0.00",
//"maxAmount":"0.00"},"partnerServiceId":"7828766","customerNo":"780152211",
//"virtualAccountNo":"7828766780152211","virtualAccountName":"Kevin H",
//"trxId":"SS-466","totalAmount":{"value":"0.00","currency":"IDR"},
//"virtualAccountTrxType":"O","expiredDate":"2025-08-23T12:57:24+07:00"},
//"responseCode":"2002700",
//"responseMessage":"Successful"}