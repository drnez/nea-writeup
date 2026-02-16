using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

class Server
{
    public void Run(string[] args)
    {
        int port;

        if (args.Length < 2 || !int.TryParse(args[1], out port))
        {
            Console.WriteLine("Provide an integer port as the second argument!");
            return;
        }

        IPAddress ip;
        if (args.Length > 2 && args[2] == "loop") ip = IPAddress.Loopback;
        else ip = IPAddress.Any;

        TcpListener listener = new TcpListener(ip, port);
        listener.Start();

        Console.WriteLine("IP: " + ip + " Port: " + port);

        Aes aes;

        while (true)
        {
            using (NetworkStream stream = listener.AcceptTcpClient().GetStream())
            {
                aes = SetupAes.Server(stream);

                FileReceiver receiver = new FileReceiver(stream, aes);

                int fileNum = receiver.GetInt();

                for (int i = 0; i < fileNum; i++) // client sending files
                {
                    receiver.GetFile("ServerReceived");
                }

                if (fileNum == 0) // client retrieving files
                {
                    string paths = receiver.GetString();

                    if (paths == "")
                    {
                        Console.WriteLine("Corrupted transmission!");
                        continue;
                    }

                    FileSender sender = new FileSender(stream, aes);

                    foreach (string path in paths.Trim().Split(','))
                    {
                        try
                        {
                            byte[] fileBytes = File.ReadAllBytes("ServerReceived" + path);

                            sender.SendFile(fileBytes, path);
                        }
                        catch
                        {
                            sender.SendFile(
                                System.Text.Encoding.ASCII.GetBytes
                                ("File not found!"), path);

                            Console.WriteLine(path + " not found!");
                        }
                    }

                    sender.Finish();
                }
            }
        }
    }
}
