using SnapApiScratch.Contracts.Models.VirtualAccount;

namespace SnapApiScratch.Storage
{
    public static class VirtualAccountStorage
    {
        public static List<CreateSnapVaResponse> Accounts { get; set; } = new List<CreateSnapVaResponse>();
    }
}
