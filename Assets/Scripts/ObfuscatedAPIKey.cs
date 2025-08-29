using UnityEngine;

public class ObfuscatedAPIKey
{
    public static string GetAPIKey()
    {
        // Load obfuscated key from resources
        TextAsset keyFile = Resources.Load<TextAsset>("obfuscated_key");
        if (keyFile == null) return "";

        return DeobfuscateKey(keyFile.text);
    }

    private static string DeobfuscateKey(string obfuscatedKey)
    {
        // Simple deobfuscation - reverse and decode
        byte[] data = System.Convert.FromBase64String(obfuscatedKey);
        System.Array.Reverse(data);

        // XOR with a constant
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(data[i] ^ 0x5A);
        }

        return System.Text.Encoding.UTF8.GetString(data);
    }

    // Helper method to create the obfuscated key (run this once)
    public static string ObfuscateKey(string originalKey)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(originalKey);

        // XOR with constant
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(data[i] ^ 0x5A);
        }

        // Reverse and encode
        System.Array.Reverse(data);
        return System.Convert.ToBase64String(data);
    }
}