using System.Net.Sockets;
using System.Security.Cryptography;

class Client
{
    private static readonly IReadOnlyList<string> _textFileExtensions = new string[]
    {
        "123","1st","600","602","890","a","ab2","ab3","abc","abw", "yml","ynab","yum","zabw","zeg","zig","zrtf","zsh","zw"
    }; // large list of plaintext file extensions - most have been removed for this writeup for clarity
    private static readonly IReadOnlyList<string> _otherFileExtensions = new string[]
    {
        "exe","dll","so","bin","msi","app","dmg","zip","rar","7z", "gz","tar","tgz","bz2","xz","iso","jpg","jpeg","png","gif"
    }; // large list of other file extensions - most have been removed for this writeup for clarity

    public void SendFiles(string[] args)
    {
        TcpClient client = SetupClient(args);
        NetworkStream stream = client.GetStream();

        List<string> textFiles = new List<string>();
        List<string> otherFiles = new List<string>();

        if (args.Length > 3)
        {
            SortFiles(textFiles, otherFiles, args);
        }
        else
        {
            Console.WriteLine("Enter file paths to send (blank line to finish)");
            GetFileInput(textFiles, otherFiles);
        }

        using (client)
        using (stream)
        {
            Aes aes = SetupAes.Client(stream);

            FileSender sender = new FileSender(stream, aes);

            List<Tuple<byte[], string>> filesToSend = new List<Tuple<byte[], string>>();

            foreach (string path in textFiles)
            {
                string fileText = "";

                try
                {
                    fileText = File.ReadAllText(path);
                }
                catch
                {
                    Console.WriteLine(path + " is invalid, so not sent!");
                    continue;
                }

                filesToSend.Add(new Tuple<byte[], string>
                        (HuffmanEncoder.WriteCompressedFile(fileText), path + ".huff"));
            }

            foreach (string path in otherFiles)
            {
                byte[] fileBytes;

                try
                {
                    fileBytes = File.ReadAllBytes(path);
                }
                catch
                {
                    Console.WriteLine(path + " is invalid, so not sent!");
                    continue;
                }

                filesToSend.Add(new Tuple<byte[], string>(fileBytes, path));
            }

            sender.SendInt(filesToSend.Count); // no. of files to expect

            foreach (Tuple<byte[], string> file in filesToSend) // sent after so non-existent files don't contribute to count
            {
                Console.WriteLine("Sending " + file.Item2);
                sender.SendFile(file.Item1, file.Item2);
            }

            sender.Finish();
        }
    }

    public void RequestFiles(string[] args)
    {
        TcpClient client = SetupClient(args);
        NetworkStream stream = client.GetStream();

        using (client)
        using (stream)
        {
            Aes aes = SetupAes.Client(stream);
            FileSender sender = new FileSender(stream, aes);

            sender.SendInt(0); // indicates desire to receive files

            List<string> textFiles = new List<string>();
            List<string> otherFiles = new List<string>();

            if (args.Length > 3)
            {
                SortFiles(textFiles, otherFiles, args);
            }
            else
            {
                Console.WriteLine("Enter file paths to receive (blank line to finish)");
                GetFileInput(textFiles, otherFiles);
            }

            string paths = "";

            foreach (string path in textFiles) paths += path + ".huff" + ",";
            foreach (string path in otherFiles) paths += path + ",";

            paths = paths.Remove(paths.Length - 1);

            sender.SendData(paths);

            sender.Finish();

            FileReceiver receiver = new FileReceiver(stream, aes);

            for (int i = 0; i < textFiles.Count + otherFiles.Count; i++)
            {
                receiver.GetFile("ClientReceived", true);
            }
        }
    }

    private TcpClient SetupClient(string[] args)
    {
        string serverIp = "";
        int serverPort;

        if (args.Length > 1)
        {
            serverIp = args[1];
        }
        if (args.Length <= 1 || !ValidateIP(serverIp)) // write this please
        {
            do
            {
                Console.Write("Enter the server's IP address: ");
                serverIp = Console.ReadLine() ?? "127.0.0.1";
            } while (!ValidateIP(serverIp));
        }

        if (!(args.Length > 2 && int.TryParse(args[2], out serverPort)))
        {
            Console.Write("Enter the server's port: ");

            while (!int.TryParse(Console.ReadLine(), out serverPort))
            {
                Console.WriteLine("Invalid port! It must be an integer! Try again.");
            }
        }

        try
        {
            return new TcpClient(serverIp, serverPort);
        }
        catch
        {
            Console.WriteLine("Error! Server not reached!");
            return SetupClient(new string[]{}); // forces restart in interactive mode
        }
    }

    private bool ValidateIP(string ipAddress)
    {
        if (!ipAddress.Contains('.')) return false;

        string[] splitIp = ipAddress.Split('.');

        if (splitIp.Length != 4) return false;

        foreach (string str in splitIp)
        {
            int ipPart;

            if (!int.TryParse(str, out ipPart)) return false;

            if (ipPart > 255 || ipPart < 0) return false;
        }

        return true;
    }

    private void GetFileInput(List<string> textFiles, List<string> otherFiles) // interactive
    {
        while (true)
        {
            string input = Console.ReadLine() ?? "";

            if (input == "") break;

            if (IsTextFile(input, true)) textFiles.Add(input);
            else otherFiles.Add(input);

        }
    }

    private void SortFiles(List<string> textFiles, List<string> otherFiles, string[] args) // non-interactive
    {
        for (int i = 3; i < args.Length; i++)
        {
            string fileName = args[i];
            string flag = (i + 1 < args.Length) ? args[i+1].ToLower() : "";

            if (flag == "t")
            {
                textFiles.Add(fileName);
                i++;
                continue;
            }
            if (flag == "o")
            {
                otherFiles.Add(fileName);
                i++;
                continue;
            }

            if (IsTextFile(fileName, false)) textFiles.Add(fileName);
            else otherFiles.Add(fileName);
        }
    }

    private bool IsTextFile(string fileName, bool interactive)
    {
        string extension;

        if (!fileName.Contains('.')) extension = "";
        else extension = fileName.Split('.').Last();

        if (_textFileExtensions.Contains(extension)) return true;
        if (_otherFileExtensions.Contains(extension)) return false;

        if (!interactive) return false;

        Console.Write("Plaintext? (y/N) ");
        if ((Console.ReadLine() ?? "").ToLower() == "y") return true;
        
        return false;
    }
}
