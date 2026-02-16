using System.Numerics;
using System.Net.Sockets;

static class Transmission
{
    public static byte[] Serialise(byte[] bytes)
    {
        byte[] bytesLengthBytes = BitConverter.GetBytes(bytes.Length); // length 4 always

        byte[] data = bytesLengthBytes.ToList().Concat(bytes.ToList()).ToArray();

        return data;
    }

    public static byte[] Serialise(BigInteger bigInt)
    {
        byte[] bigIntBytes = bigInt.ToByteArray(true, true); // isUnsigned, isBigEndian

        return Serialise(bigIntBytes);
    }

    public static byte[] Serialise(string data) => Serialise(System.Text.Encoding.ASCII.GetBytes(data));

    public static BigInteger ReadBigInt(NetworkStream stream) => new BigInteger(ReadByteArray(stream), true, true);

    public static byte[] ReadByteArray(Stream stream)
    {
        int bytesLength = BitConverter.ToInt32(ReadBytes(stream, 4));

        return ReadBytes(stream, bytesLength);
    }

    public static byte[] ReadBytes(Stream stream, int length)
    {
        byte[] bytes = new byte[length];

        int read = 0;
        while (read < length) read += stream.Read(bytes, read, length - read);

        return bytes;
    }
}
