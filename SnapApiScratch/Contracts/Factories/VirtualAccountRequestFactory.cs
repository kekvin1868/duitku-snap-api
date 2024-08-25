using SnapApiScratch.Contracts.Helpers;
using SnapApiScratch.Contracts.Helpers.HelperModels;
using SnapApiScratch.Contracts.Models.VirtualAccount;
using System;
using System.Text;
using System.Text.Json;

namespace SnapApiScratch.Contracts.Factories
{
    public class VirtualAccountRequestFactory
    {
        private readonly GetBearerToken _bearerTokenProvider;

        public VirtualAccountRequestFactory(GetBearerToken getBearerToken)
        {
            _bearerTokenProvider = getBearerToken;
        }
        public StringContent CreateRequest()
        {
            var random = new Random();
            DateTime dateTime = DateTime.Now.AddDays(1);
            string formattedDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss") + "+0700";

            CreateSnapVaRequest reqContent = new CreateSnapVaRequest {
                PartnerServiceId = "7828766",
                CustomerNo = $"{random.Next(1, 1000000000)}",
                VirtualAccountName = "Kevin H",
                TrxId = $"SUMISHOP-{random.Next(1, 10000)}",
                ExpiredDate = $"{formattedDateTime}",
                TotalAmount = {
                    Value = "0",
                    Currency = "IDR"
                }
            };

            reqContent.VirtualAccountNo = $"{reqContent.PartnerServiceId}{reqContent.CustomerNo}";
            int checkTotalAmount = int.Parse(reqContent.TotalAmount.Value);

            if (checkTotalAmount == 0)
            {
                reqContent.VirtualAccountTrxType = "O";
            }

            StringContent content = new StringContent(
                JsonSerializer.Serialize(reqContent),
                Encoding.UTF8,
                "application/json"
            );

            return content;
        }

        // public async Task<HttpRequestMessage> CreateRequestHeaders (HttpRequestMessage currentHttpReq)
        // {
        //     ReturnTokenModel? apiTokenModel = await _bearerTokenProvider.GetBearerTokenAsync();
            
        //     currentHttpReq.Headers.Add("Authorization", $"Bearer {apiTokenModel?.Signature}");

        //     return currentHttpReq;
        // }
    }
}