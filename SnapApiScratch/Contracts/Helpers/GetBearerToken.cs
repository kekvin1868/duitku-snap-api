using SnapApiScratch.Contracts.Helpers.HelperModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Globalization;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SnapApiScratch.Contracts.Helpers
{
    public class GetBearerToken
    {
        private readonly HttpClient _httpClient;
        private readonly string _partnerId;
        private readonly string _privateKeyPath;
        public GetBearerToken(HttpClient httpClient, string partnerId, string privateKeyPath)
        {
            _httpClient = httpClient;
            _partnerId = partnerId;
            _privateKeyPath = privateKeyPath;
        }
        public async Task<HttpRequestMessage> GetAllHeaders(HttpRequestMessage req)
        {   
            var random = new Random();
            var externalId = random.Next(10000, 100000).ToString("D31");
            var timestamp = DateTimeOffset.Now
                .ToOffset(TimeSpan.FromHours(7))
                .ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            var partnerId = "DS19890";
            
            // CREATE VA
            // string endpoint = "/merchant/va/v1.0/transfer-va/create-va";
            
            // UPDATE VA 
            // string endpoint = "/merchant/va/v1.0/transfer-va/update-va";

            // INQUIRY VA
            // string endpoint = "/merchant/va/v1.0/transfer-va/inquiry-va";

            // DELETE VA
            // string endpoint = "/merchant/va/v1.0/transfer-va/delete-va";

            // INQUIRY STATUS VA
            // string endpoint = "/merchant/va/v1.0/transfer-va/status";

            // ACCOUNT BINDING DEBIT
            // string endpoint = "/merchant/registration/v1.0/registration-account-binding";

            // DEBIT PAYMENT REDIRECT
            // string endpoint = "/merchant/debit/v1.0/debit/payment-host-to-host";

            // DEBIT PAYMENT STATUS
            // string endpoint = "/merchant/debit/v1.0/debit/status";

            // DEBIT PAYMENT QRIS
            // string endpoint = "/merchant/qris/v1.0/qr/qr-mpm-generate";

            // DEBIT QUERY PAYMENT
            string endpoint = "/merchant/qris/v1.0/qr/qr-mpm-query";

            //7828766
            var tokenResult = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            ReturnTokenModel? tokenObject = JsonSerializer.Deserialize<ReturnTokenModel>(tokenResult);
        
            if (tokenObject == null || tokenObject.accessToken == null || req.Content == null)
            {
                throw new InvalidOperationException("Token object or access token is null");
            }

            string accessToken = tokenObject.accessToken;
            string bodyJson = req.Content.ReadAsStringAsync().Result;

            // POST
            string signature = GenerateSymmetricSignature("POST", endpoint, accessToken, bodyJson, timestamp);

            // PUT
            // string signature = GenerateSymmetricSignature("PUT", endpoint, accessToken, bodyJson, timestamp);

            // DELETE
            // string signature = GenerateSymmetricSignature("DELETE", endpoint, accessToken, bodyJson, timestamp);

            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject?.accessToken);
            req.Headers.Add("X-TIMESTAMP", timestamp);
            req.Headers.Add("X-PARTNER-ID", partnerId);
            req.Headers.Add("X-SIGNATURE", signature);
            req.Headers.Add("X-EXTERNAL-ID", externalId);
            req.Headers.Add("CHANNEL-ID", "SP");

            return req;
        }

        private async Task<string> ExecNodeJsScriptAsync(string scriptPath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start the Node.js process.");
            }

            using var reader = process.StandardOutput;
            var result = await reader.ReadToEndAsync();
            process.WaitForExit();

            // Check for errors
            using var errorReader = process.StandardError;
            var errors = await errorReader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(errors))
            {
                throw new InvalidOperationException($"Node.js script error: {errors}");
            }

            return result;
        }

        private string GenerateSymmetricSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "6a9b62c66ed1b89c845fa855514bfcae";
            byte[] hashBytes;
            string bodyJsonHexString;

            using (SHA256 sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(requestBodyJson));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                bodyJsonHexString = sb.ToString();
            }

            string stringToSign = $"{httpMethod}:{endpointUrl}:{accessToken}:{bodyJsonHexString}:{timestamp}";

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes($"{secret}"));
            var signatureHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

            string signature = Convert.ToBase64String(signatureHashBytes);

            return signature;
        }
    }
}