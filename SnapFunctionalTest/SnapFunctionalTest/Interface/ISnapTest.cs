using Refit;
using SnapFunctionalTest.Contracts;

namespace SnapFunctionalTest.Interface;

public interface ISnapTest
{
    [Headers("CHANNEL-ID: DUITKU")]
    [Post("/v1.0/transfer-va/payment")]
    Task<ApiResponse<SnapCreateVaResponse>> PaymentVa(
        [Body] SnapCreateVaRequest request,
        [Header("X-PARTNER-ID")] string partnerId,
        [Header("X-EXTERNAL-ID")] string externalId);
}