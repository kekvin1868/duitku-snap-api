using System.Security.Cryptography;
using System.Text;

namespace SnapFunctionalTest.Helpers;

public static class CryptoHelper
{
    public static string SHA256WithRSA(string data, string privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signatureBytes);
    }
    
    public static bool VerifySHA256WithRSA(string data, string signature, string publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var signatureBytes = Convert.FromBase64String(signature);
        return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
    
    public static string HMACSHA512(string data, string key)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public static string Sha256(string data)
    {
        // Create a SHA256   
        using var sha256 = SHA256.Create();
        // ComputeHash - returns byte array  
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        // Convert byte array to a string   
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}