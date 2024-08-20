using Refit;
using SnapFunctionalTest.Contracts;

namespace SnapFunctionalTest.Interface;

public interface ISnapTokenApi
{
    [Post("/auth/v1.0/access-token/b2b")]
    Task<ApiResponse<SnapTokenResponse>> GetToken(
        [Body] SnapTokenRequest request, 
        [Header("X-TIMESTAMP")] string timestamp,
        [Header("X-CLIENT-KEY")] string clientKey,
        [Header("X-SIGNATURE")] string signature);
}