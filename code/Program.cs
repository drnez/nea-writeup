bool Program()
{
    if (args.Length < 3) return false;

    switch (args[0] ?? "")
    {
        case "-e":
            string fileText;

            try
            {
                fileText = File.ReadAllText(args[1]);
            }
            catch
            {
                Console.WriteLine("Could not read from " + args[1]);
                return true;
            }

            HuffmanEncoder.WriteCompressedFile(fileText, args[2]);
            break;

        case "-d":
            byte[] fileBytes;

            try
            {
                fileBytes = File.ReadAllBytes(args[1]);
            }
            catch
            {
                Console.WriteLine("Could not read from " + args[1]);
                return true;
            }

            HuffmanDecoder.WriteRawFile(fileBytes, args[2]);
            break;

        default:
            return false;
    }

    return true;
}

if (!Program())
    Console.WriteLine("Error! Format:\n-[e,d] [input file] [output path]");
