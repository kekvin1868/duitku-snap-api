using FastEndpoints;
using Microsoft.Extensions.Options;
using SnapFunctionalTest.Contracts;
using SnapFunctionalTest.Interface;

namespace SnapFunctionalTest.Features.DeleteVa;

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
        Get("/delete-va");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string partnerServiceId = "7828766";
        string customerId = "1234567891";
        string virtualAccountNo = $"{partnerServiceId}{customerId}";
        string trxId = "QP-Create-2";

        var requestParam = SnapVaRequestFactory.CreateDeleteRequest(partnerServiceId,
            customerId,
            virtualAccountNo,
            trxId);

        try
        {
            var requestSnap = await _snapVaApi.DeleteVa(requestParam,
                _options.Value.ClientId,
                Guid.NewGuid().ToString("N"));

            var response = requestSnap.Content;

            await SendAsync(response, cancellation: ct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}