using Refit;
using SnapFunctionalTest.Contracts;

namespace SnapFunctionalTest.Interface;

public interface ISnapVaApi
{
    [Headers("CHANNEL-ID: DUITKU")]
    [Post("/merchant/va/v1.0/transfer-va/create-va")]
    Task<ApiResponse<SnapCreateVaResponse>> CreateVa(
        [Body] SnapCreateVaRequest request,
        [Header("X-PARTNER-ID")] string partnerId,
        [Header("X-EXTERNAL-ID")] string externalId);
    
    [Headers("CHANNEL-ID: DUITKU")]
    [Put("/merchant/va/v1.0/transfer-va/update-va")]
    Task<ApiResponse<SnapCreateVaResponse>> UpdateVa(
        [Body] SnapCreateVaRequest request,
        [Header("X-PARTNER-ID")] string partnerId,
        [Header("X-EXTERNAL-ID")] string externalId);
    
    [Headers("CHANNEL-ID: DUITKU")]
    [Post("/merchant/va/v1.0/transfer-va/inquiry-va")]
    Task<ApiResponse<SnapInquiryVaResponse>> InquiryVa(
        [Body] SnapInquiryVaRequest request,
        [Header("X-PARTNER-ID")] string partnerId,
        [Header("X-EXTERNAL-ID")] string externalId);
    
    [Headers("CHANNEL-ID: DUITKU")]
    [Delete("/merchant/va/v1.0/transfer-va/delete-va")]
    Task<ApiResponse<SnapDeleteVaResponse>> DeleteVa(
        [Body] SnapDeleteVaRequest request,
        [Header("X-PARTNER-ID")] string partnerId,
        [Header("X-EXTERNAL-ID")] string externalId);
}