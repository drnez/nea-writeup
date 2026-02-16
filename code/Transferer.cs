using System.Net.Sockets;
using System.Security.Cryptography;

abstract class Transferer
{
    protected CryptoStream _cryptoStream;

    public Transferer(NetworkStream stream, Aes aes, CryptoStreamMode mode)
    {
        aes.Padding = PaddingMode.Zeros;

        _cryptoStream = new CryptoStream(stream, mode == CryptoStreamMode.Write ? aes.CreateEncryptor() : aes.CreateDecryptor(), mode, leaveOpen: true);
    }
}

class FileSender : Transferer
{
    public FileSender(NetworkStream stream, Aes aes) : base(stream, aes, CryptoStreamMode.Write) { }

    public void SendFile(byte[] fileBytes, string fileName)
    {
        _cryptoStream.Write(
                Transmission.Serialise(Hashing.Hash(fileBytes))
            );

        _cryptoStream.Write(Transmission.Serialise(fileName));

        SendData(fileBytes);
    }

    public void SendData(string data)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);

        _cryptoStream.Write(
                Transmission.Serialise(Hashing.Hash(bytes))
            );

        SendData(bytes);
    }

    public void SendInt(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data); // 4 bytes

        SendData(bytes);
    }

    private void SendData(byte[] bytes)
    {
        _cryptoStream.Write(Transmission.Serialise(bytes));
        _cryptoStream.Flush();
    }

    public void Finish() => _cryptoStream.FlushFinalBlock();
}

class FileReceiver : Transferer
{
    public FileReceiver(NetworkStream stream, Aes aes) : base(stream, aes, CryptoStreamMode.Read) { }

    public void GetFile(string location, bool decompress = false)
    {
        byte[] hashBytes = Transmission.ReadByteArray(_cryptoStream); // 32 byte hash read
        byte[] nameBytes = Transmission.ReadByteArray(_cryptoStream); // read file name
        byte[] fileBytes = Transmission.ReadByteArray(_cryptoStream);

        if (Hashing.VerifyHash(fileBytes, hashBytes))
        {
            string name = System.Text.Encoding.ASCII.GetString(nameBytes);

            string filePath = location + name;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");

            if (decompress && name.Split('.').LastOrDefault() == "huff")
            {
                HuffmanDecoder.WriteRawFile(fileBytes, filePath.Substring(0, filePath.Length - 5));
            }
            else File.WriteAllBytes(filePath, fileBytes);
        }
        else Console.WriteLine("Corrupted transmission!");
    }

    public string GetString()
    {
        byte[] hashBytes = Transmission.ReadByteArray(_cryptoStream);
        byte[] stringBytes = Transmission.ReadByteArray(_cryptoStream);

        if (Hashing.VerifyHash(stringBytes, hashBytes))
        {
            return System.Text.Encoding.ASCII.GetString(stringBytes);
        }

        return "";
    }

    public int GetInt()
    {
        byte[] intBytes = Transmission.ReadByteArray(_cryptoStream);

        return BitConverter.ToInt32(intBytes);
    }
}
