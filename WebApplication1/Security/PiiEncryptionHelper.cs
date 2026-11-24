using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Security;

public static class PiiEncryptionHelper
{
    private static byte[] _aesKey = Array.Empty<byte>();
    private static byte[] _hmacKey = Array.Empty<byte>();
    private static bool _isEncryptionEnabled = true;
    private const string Prefix = "ENC::";

    public static void Initialize(string aesKeyString, string hmacKeyString, bool enableEncryption = true)
    {
        _isEncryptionEnabled = enableEncryption;

        if (string.IsNullOrWhiteSpace(hmacKeyString) || hmacKeyString.Length < 32)
            throw new InvalidOperationException("BLIND_INDEX_KEY must be at least 32 characters.");

        _hmacKey = SHA256.HashData(Encoding.UTF8.GetBytes(hmacKeyString));

        if (_isEncryptionEnabled)
        {
            if (string.IsNullOrWhiteSpace(aesKeyString) || aesKeyString.Length < 32)
                throw new InvalidOperationException("DATA_ENCRYPTION_KEY must be at least 32 characters.");

            _aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(aesKeyString));
        }
    }

    // 🔒 EnablePiiEncryption == false නම් Plain Text එකම save වේ
    public static string? Encrypt(string? plainText)
    {
        if (!_isEncryptionEnabled || string.IsNullOrEmpty(plainText))
            return plainText;

        if (plainText.StartsWith(Prefix))
            return plainText;

        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var combined = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

        return Prefix + Convert.ToBase64String(combined);
    }

    // 🔓 Plain text තිබුණත් Encrypted (ENC::) තිබුණත් දෙකම safe විදිහට කියවයි
    public static string? Decrypt(string? cipherText)
    {
        if (string.IsNullOrEmpty(cipherText) || !cipherText.StartsWith(Prefix))
            return cipherText;

        if (_aesKey.Length == 0)
            return cipherText;

        var rawData = Convert.FromBase64String(cipherText[Prefix.Length..]);
        using var aes = Aes.Create();
        aes.Key = _aesKey;

        var iv = new byte[16];
        var cipherBytes = new byte[rawData.Length - 16];
        Buffer.BlockCopy(rawData, 0, iv, 0, 16);
        Buffer.BlockCopy(rawData, 16, cipherBytes, 0, cipherBytes.Length);

        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    // 🔍 Blind Index (Development සහ Production දෙකේදීම Search queries වැඩ කිරීමට)
    public static string? ComputeBlindIndex(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var normalized = value.Trim().ToLowerInvariant();
        using var hmac = new HMACSHA256(_hmacKey);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(hashBytes);
    }
}