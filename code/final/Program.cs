Client client;
Server server;

if (args.Length == 0)
{
    PrintFormatDetails();
    return;
}

switch (args[0])
{
    case "-cs":
        client = new Client();
        client.SendFiles(args);

        break;

    case "-cr":
        client = new Client();
        client.RequestFiles(args);

        break;

    case "-s":
        server = new Server();
        server.Run(args);

        break;

    default:
        PrintFormatDetails();

        break;
}

void PrintFormatDetails()
{
        Console.WriteLine("Format:");
        Console.WriteLine();
        Console.WriteLine("-cs (Client Send)    [Server IP] [Server Port] [File1] t/o (t for text file, o for other file) [File2] ...");
        Console.WriteLine("-cr (Client Receive) [Server IP] [Server Port] [File1] t/o (t for text file, o for other file) [File2] ...");
        Console.WriteLine("-s (Server) [Port] loop (enter if the server should be run on the loopback address)");
        Console.WriteLine();
        Console.WriteLine("Any details not provided will be requested interactively, but the initial flag is mandatory.");
        Console.WriteLine();
        Console.WriteLine("Note that providing any details via CLI flags depends on the previous details also being enetered: you cannot pass files as an argument without entering the server IP and port!");
}
