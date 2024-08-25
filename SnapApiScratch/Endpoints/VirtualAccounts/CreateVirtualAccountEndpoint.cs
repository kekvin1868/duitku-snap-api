using SnapApiScratch.Storage;
using SnapApiScratch.Contracts.Models.VirtualAccount;
using SnapApiScratch.Contracts.Factories;
using FastEndpoints;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;


namespace SnapApiScratch.Endpoints.VirtualAccounts
{
    public class CreateVirtualAccountEndpoint : EndpointWithoutRequest
    {
        private readonly HttpClient _client;
        private readonly VirtualAccountRequestFactory _virtualAccountReqFactory;

        public CreateVirtualAccountEndpoint(IHttpClientFactory httpClientFactory, VirtualAccountRequestFactory virtualAccountReqFactory)
        {
            _client = httpClientFactory.CreateClient("DuitkuClient");
            _virtualAccountReqFactory = virtualAccountReqFactory;
        }

        public override void Configure()
        {
            Get("/va-good-structure");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            StringContent requestBody = _virtualAccountReqFactory.CreateRequest();

            try {
                HttpRequestMessage initialRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/merchant/va/v1.0/transfer-va/create-va")
                {
                    Content = requestBody
                };

                // HttpRequestMessage finalRequest = await _virtualAccountReqFactory.CreateRequestHeaders(initialRequest);

                // var response = await _client.SendAsync(finalRequest);
                // string responseContent = await response.Content.ReadAsStringAsync();
                
                // if (response.IsSuccessStatusCode)
                // {
                //     CreateSnapVaResponse? createSnapVaResponse = JsonSerializer.Deserialize<CreateSnapVaResponse>(responseContent);

                //     if (createSnapVaResponse != null)
                //     {
                //         await SendAsync(createSnapVaResponse);
                //     }
                //     else
                //     {
                //         Console.WriteLine("Failed result null.");
                //         await SendErrorsAsync(500);
                //     }
                // }
                // else
                // {
                //     Console.WriteLine(responseContent);
                //     Console.WriteLine("Failed here on CreateVirtualAccountEndpoint.cs");
                //     await SendErrorsAsync(500, ct);
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await SendErrorsAsync(500, ct);
            }
        }
    }
}