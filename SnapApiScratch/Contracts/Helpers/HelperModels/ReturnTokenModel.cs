namespace SnapApiScratch.Contracts.Helpers.HelperModels
{
    public class ReturnTokenModel {
        public string? responseCode { get; set; }
        public string? responseMessage { get; set; }
        public string? accessToken { get; set; }
        public string? tokenType { get; set; }
        public string? expiresIn { get; set; }
    }
}