using SnapApiScratch.Endpoints.AllPaymentGateway;
using FastEndpoints;
using System.Text.Json;


namespace SnapApiScratch.Endpoints.AllPaymentGateway
{
    public class CreateVaEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public CreateVaEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/create-va");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/va/v1.0/transfer-va/inquiry-va")
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

    public class UpdateVaEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public UpdateVaEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/update-va");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Put, "/merchant/va/v1.0/transfer-va/update-va")
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

    public class InquiryVaEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public InquiryVaEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/inquiry-va");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/va/v1.0/transfer-va/inquiry-va")
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

    public class DeleteVaEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public DeleteVaEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/delete-va");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Delete, "/merchant/va/v1.0/transfer-va/delete-va")
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

    public class InquiryStatusEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly GetAllPgRequests _getAllPgRequests;

        public InquiryStatusEndpoint(IHttpClientFactory httpClientFactory, GetAllPgRequests getAllPgRequests)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _getAllPgRequests = getAllPgRequests;
        }

        public override void Configure()
        {
            Get("/status-va");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _getAllPgRequests.CreateVaRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/va/v1.0/transfer-va/status")
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