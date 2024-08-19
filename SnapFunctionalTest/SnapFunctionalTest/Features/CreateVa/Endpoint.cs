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
        string customerId = "1234567891";
        string virtualAccountNo = $"{partnerServiceId}{customerId}";
        string vaName = "John Doe";
        string trxId = "QP-Create-3";
        string expiredDate = DateTime.Now.AddDays(1).ToString("O");

        var requestParam = SnapVaRequestFactory.CreateRequest(partnerServiceId,
            customerId,
            virtualAccountNo,
            vaName,
            trxId,
            0);

        try
        {
            var requestSnap = await _snapVaApi.CreateVa(requestParam,
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