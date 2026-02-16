using System.Security.Cryptography;

static class Hashing
{ 
    public static byte[] Hash(byte[] messageBytes)
    { 
        byte[] hashBytes;

        using (SHA256 HashMaker = SHA256.Create())
        {
            hashBytes = HashMaker.ComputeHash(messageBytes);
        }

        return hashBytes;   
    }

    public static bool VerifyHash(byte[] messageBytes, byte[] hash)
    {
        byte[] originalHashed = Hash(messageBytes);

        if (originalHashed.Length != hash.Length) return false;

        for (int i = 0; i < hash.Length; i++)
        {
            if (originalHashed[i] != hash[i]) return false;
        }

        return true;
    }
}
