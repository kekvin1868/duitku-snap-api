using FastEndpoints;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Security.Cryptography;


namespace SnapApiScratch.Endpoints.Disbursements
{
    public class SnapDisbursementEndpoint : EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementEndpoint(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/create-inquiry-external");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 10000000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/account-inquiry-external";

                // Create request headers
                string timestamp = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                var requestBody = new
                {
                    beneficiaryAccountNo = "8760673566",
                    partnerReferenceNo = partnerRefNo,
                    beneficiaryBankCode = "014",
                    additionalInfo = new
                    {
                        billType = "SKN",
                        amount = new
                        {
                            value = "100000000.00",
                            currency = "IDR"
                        }
                    }
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("External Inquiry Made at: " + timestamp);
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize the response.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementInquiryInternal : EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementInquiryInternal(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/create-inquiry-internal");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 10000000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/account-inquiry-internal";

                // Create request headers
                var timestamp = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                var requestBody = new
                {
                    beneficiaryAccountNo = "8760673566",
                    partnerReferenceNo = partnerRefNo,
                    additionalInfo = new
                    {
                        amount = new
                        {
                            value = "80000.00",
                            currency = "IDR"
                        },
                        beneficiaryBankCode = "014"
                    }
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(timestamp);
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize the response.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementTrf : EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementTrf(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/transfer-interbankz");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 10000000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/transfer-interbank";

                // Create request headers
                var timestamp = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // INTERBANK
                var requestBody = new
                {
                    partnerReferenceNo = "SS7936",
                    beneficiaryAccountNo = "8760673566",
                    beneficiaryBankCode = "002",
                    transactionDate = timestamp,
                    amount = new
                    {
                        value = "50000.00",
                        currency = "IDR"
                    },
                    sourceAccountNo = "19993",
                    additionalInfo = new
                    {
                        remark = "CoGS 22-08-2024"
                    }
                };

                // INTRABANK
                // var requestBody = new
                // {
                //     partnerReferenceNo = "SS4527",
                //     beneficiaryAccountNo = "8760673566",
                //     transactionDate = timestamp,
                //     amount = new
                //     {
                //         value = "50000.00",
                //         currency = "IDR"
                //     },
                //     sourceAccountNo = "19993",
                //     additionalInfo = new
                //     {
                //         beneficiaryBankCode = "013"
                //     }
                // };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementCheckTrf : EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementCheckTrf(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/check-transfer");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/transfer/status";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status
                var requestBody = new
                {
                    originalPartnerReferenceNo = "SS7936",
                    serviceCode = "18"
                };
                // originalReferenceNo = "614588",

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementRtgs : EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementRtgs(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/transfer-rtgs");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/transfer-rtgs";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status
                var requestBody = new
                {
                    partnerReferenceNo = "SS7847",
                    beneficiaryAccountNo = "8760673566",
                    beneficiaryBankCode = "002",
                    sourceAccountNo = "19993",
                    transactionDate = "2024-08-22T16:51:01+07:00",
                    beneficiaryCustomerResidence = "1",
                    beneficiaryCustomerType = "1",
                    amount = new 
                    {
                        value = "100000000.00",
                        currency = "IDR"
                    },
                    additionalInfo = new
                    {
                        remark = "Government Business Goods: 22-08-2024"
                    }
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("RTGS API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementSknbi : EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementSknbi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/transfer-sknbi");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/transfer-skn";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status
                var requestBody = new
                {
                    partnerReferenceNo = "SS1633",
                    beneficiaryAccountNo = "8760673566",
                    beneficiaryBankCode = "014",
                    sourceAccountNo = "19993",
                    transactionDate = timestamp,
                    beneficiaryCustomerResidence = "1",
                    beneficiaryCustomerType = "3",
                    amount = new 
                    {
                        value = "100000000.00",
                        currency = "IDR"
                    },
                    additionalInfo = new
                    {
                        remark = "Government Business Goods: 22-08-2024"
                    }
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementCheckBalance: EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementCheckBalance(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/check-balance");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SS" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/merchant/account/balance";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status DS19890
                var requestBody = new
                {
                    accountNo = "19993"
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementTopUpInquiry: EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementTopUpInquiry(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/topup-inquiry");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SSTOPUP" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/emoney/account-inquiry";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status DS19890
                var requestBody = new
                {
                    partnerReferenceNo = partnerRefNo,
                    customerNumber = "6285718159111",
                    amount = new
                    {
                        value = "100000.00",
                        currency = "IDR"
                    },
                    additionalInfo = new
                    {
                        beneficiaryBankCode = "1013"
                    }
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementTopUp: EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementTopUp(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/topup");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SSTOPUP" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/emoney/topup";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status DS19890
                var requestBody = new
                {
                    partnerReferenceNo = "SSTOPUP555",
                    customerNumber = "6285718159111",
                    amount = new
                    {
                        value = "100000.00",
                        currency = "IDR"
                    },
                    additionalInfo = new
                    {
                        beneficiaryBankCode = "1013",
                        remark = "ShoppeePay Topup Test, 22-08-2024"
                    }
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class SnapDisbursementTopUpStatus: EndpointWithoutRequest
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SnapDisbursementTopUpStatus(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override void Configure()
        {
            Get("/topup-status");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("DuitkuClient");

            var result = await ExecNodeJsScriptAsync("/Users/kevin/Desktop/token.js");
            BearerTokenResponse? tokenObject = JsonSerializer.Deserialize<BearerTokenResponse>(result);


            if (tokenObject != null && tokenObject?.accessToken != null)
            {
                var random = new Random();
                var externalId = random.Next(10000, 100000).ToString("D31");
                var partnerRefNo = "SSTOPUP" + random.Next(1, 10000);
                var channelId = random.Next(10000, 99999);
                var endpoint = "db/snap/v1.0/emoney/topup-status";

                // Create request headers
                var timestamp = DateTimeOffset.Now
                    .ToOffset(TimeSpan.FromHours(7))
                    .ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                var partnerId = "DBS19993";

                // Check Status DS19890
                var requestBody = new
                {
                    originalPartnerNo = "SSTOPUP555",
                    originalReferenceNo = "615725",
                    serviceCode = "38"
                };

                var requestBodyJson = JsonSerializer.Serialize(requestBody);

                var signature = GenerateSignature("POST", "/" + endpoint, tokenObject.accessToken, requestBodyJson, timestamp);

                HttpRequestMessage request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{httpClient.BaseAddress}{endpoint}")
                {
                    Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject.accessToken);
                request.Headers.Add("X-TIMESTAMP", timestamp);
                request.Headers.Add("X-PARTNER-ID", partnerId);
                request.Headers.Add("X-SIGNATURE", signature);
                request.Headers.Add("X-EXTERNAL-ID", externalId);
                request.Headers.Add("CHANNEL-ID", channelId.ToString());

                // Make the API call
                var response = await httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

                // Handle the API response
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseData);
                }
                else
                {
                    Console.WriteLine(responseData);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON.");
            }
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

        private string GenerateSignature(
            string httpMethod,
            string endpointUrl,
            string accessToken,
            string requestBodyJson,
            string timestamp
        )
        {
            string secret = "45b55e333480fd0f3eeb8bbf426794caf47963f9a6e9ee6f35f6632a872ffbed";
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

    public class BearerTokenResponse
    {
        public string? responseCode { get; set; }
        public string? responseMessage { get; set; }
        public string? accessToken { get; set; }
        public string? tokenType { get; set; }
        public string? expiresIn { get; set; }
    }
}
//SS4527
//API Response: {"responseCode":"2001500","responseMessage":"Successful","referenceNo":"614047","partnerReferenceNo":"SS4527","beneficiaryAccountName":"Testing Account","beneficiaryAccountNo":"8760673566","additionalInfo":{"customerReference":"000000560869","beneficiaryBankCode":"013","beneficiaryBankName":"PT BANK PERMATA TBK","amount":{"value":"50000.00","currency":"IDR"}}}