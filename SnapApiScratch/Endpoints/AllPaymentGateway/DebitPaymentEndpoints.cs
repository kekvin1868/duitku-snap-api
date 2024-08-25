using SnapApiScratch.Endpoints.AllPaymentGateway;
using FastEndpoints;
using System.Text.Json;

namespace SnapApiScratch.Endpoints.AllPaymentGateway
{
    public class DebitAccountBindingEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public DebitAccountBindingEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/accountbinding-connect");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/registration/v1.0/registration-account-binding")
                {
                    Content = requestBody
                };

                HttpRequestMessage finalRequest = await _getAllPgRequests.CreateRequestHeaders(initialRequest);

                var response = await _client.SendAsync(finalRequest);
                string responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine(responseContent);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await SendErrorsAsync(500, ct);
            }
        }
    }

    public class DebitPaymentRedirectEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public DebitPaymentRedirectEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/debit-payment-redirect");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/debit/v1.0/debit/payment-host-to-host")
                {
                    Content = requestBody
                };

                HttpRequestMessage finalRequest = await _getAllPgRequests.CreateRequestHeaders(initialRequest);

                var response = await _client.SendAsync(finalRequest);
                string responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine(responseContent);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await SendErrorsAsync(500, ct);
            }
        }
    }

    public class DebitPaymentStatusEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public DebitPaymentStatusEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/debit-payment-status");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/debit/v1.0/debit/status")
                {
                    Content = requestBody
                };

                HttpRequestMessage finalRequest = await _getAllPgRequests.CreateRequestHeaders(initialRequest);

                var response = await _client.SendAsync(finalRequest);
                string responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine(responseContent);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await SendErrorsAsync(500, ct);
            }
        }
    }

    public class DebitPaymentQrisEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public DebitPaymentQrisEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/payment-qris");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/qris/v1.0/qr/qr-mpm-generate")
                {
                    Content = requestBody
                };

                HttpRequestMessage finalRequest = await _getAllPgRequests.CreateRequestHeaders(initialRequest);

                var response = await _client.SendAsync(finalRequest);
                string responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine(responseContent);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await SendErrorsAsync(500, ct);
            }
        }
    }

    public class DebitPaymentQueryPaymentEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public DebitPaymentQueryPaymentEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/query-payment");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/qris/v1.0/qr/qr-mpm-query")
                {
                    Content = requestBody
                };

                HttpRequestMessage finalRequest = await _getAllPgRequests.CreateRequestHeaders(initialRequest);

                var response = await _client.SendAsync(finalRequest);
                string responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API Response: " + responseContent);
                }
                else
                {
                    Console.WriteLine(responseContent);
                    Console.WriteLine("API Call Failed. Status Code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await SendErrorsAsync(500, ct);
            }
        }
    }
}