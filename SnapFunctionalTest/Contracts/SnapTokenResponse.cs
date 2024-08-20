namespace SnapFunctionalTest.Contracts;

public class SnapTokenResponse : SnapGenericResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public string ExpiresIn { get; set; }
}