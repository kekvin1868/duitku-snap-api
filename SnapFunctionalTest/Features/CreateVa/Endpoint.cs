using FastEndpoints;
using Microsoft.Extensions.Options;
using SnapFunctionalTest.Contracts;
using SnapFunctionalTest.Interface;

namespace SnapFunctionalTest.Features.CreateVa;

public class Endpoint : EndpointWithoutRequest
{
    private readonly ISnapVaApi _snapVaApi;
    private readonly IOptions<SnapSettings> _options;

    public Endpoint(ISnapVaApi snapVaApi, IOptions<SnapSettings> options)
    {
        _snapVaApi = snapVaApi;
        _options = options;
    }

    public override void Configure()
    {
        Get("/create-va");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var random = new Random();
        
        string partnerServiceId = "7828766";
        string customerId = $"{random.Next(1, 1000000000)}";
        string virtualAccountNo = $"{partnerServiceId}{customerId}";
        string vaName = "John Doe";
        string trxId = $"SS-{random.Next(1, 10000)}";

        var requestParam = SnapVaRequestFactory.CreateRequest(
            partnerServiceId,
            customerId,
            virtualAccountNo,
            vaName,
            trxId,
            0.00m);

        try
        {
            var requestSnap = await _snapVaApi.CreateVa(requestParam,
                _options.Value.ClientId,
                Guid.NewGuid().ToString("N"));

            var response = requestSnap.Content;
            Console.WriteLine(response);

            await SendAsync(response, cancellation: ct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}