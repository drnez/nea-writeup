using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;

static class SetupAes
{
    public static Aes Client(NetworkStream stream)
    {
        BigInteger n = Transmission.ReadBigInt(stream);
        BigInteger e = Transmission.ReadBigInt(stream);

        Aes aes = Aes.Create(); // creates key and IV

        BigInteger plaintext = new BigInteger(aes.Key, true, true);

        BigInteger ciphertext = BigInteger.ModPow(plaintext, e, n);
       
        stream.Write(Transmission.Serialise(ciphertext)); // sends RSA encrypted AES key
       
        aes.IV = Transmission.ReadByteArray(stream);
       
        return aes;
    }

    public static Aes Server(NetworkStream stream)
    {
        Tuple<BigInteger, BigInteger, BigInteger> keys = RSA.GenerateKeys(); // n, e, d

        stream.Write(Transmission.Serialise(keys.Item1)); // send n
        stream.Write(Transmission.Serialise(keys.Item2)); // send e

        BigInteger ciphertext = Transmission.ReadBigInt(stream); // recieve RSA encrypted AES key

        BigInteger plaintext = BigInteger.ModPow(ciphertext, keys.Item3, keys.Item1); // M = C^d (mod n)

        byte[] aesKey = plaintext.ToByteArray(true, true); // isUnsigned, isBigEndian

        Aes aes = Aes.Create();
        aes.Key = aesKey;

        aes.GenerateIV();
        stream.Write(Transmission.Serialise(aes.IV)); // send IV

        return aes;
    }
}
