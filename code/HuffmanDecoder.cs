static class HuffmanDecoder
{
    public static void WriteRawFile(byte[] bytes, string path)
    {
        bool fileCompressed;

        ByteReader byteReader = new ByteReader(bytes, out fileCompressed);
        string rawFile = "";

        if (!fileCompressed)
        {
            while (true)
            {
                char? nextChar = byteReader.ReadChar();

                if (nextChar == null) break;

                rawFile += nextChar;
            }
        }
        else
        {
            Stack<HuffmanNode> NodeStack = new Stack<HuffmanNode>();

            while (true)
            {
                int nextBit = byteReader.Read();

                if (nextBit == 1)
                {
                    char? nextChar = byteReader.ReadChar();
                    if (nextChar == null) break;

                    NodeStack.Push(new HuffmanNode((char)nextChar));
                }
                else
                {
                    if (NodeStack.Count() == 1) break;

                    HuffmanNode right = NodeStack.Pop();
                    HuffmanNode left = NodeStack.Pop();

                    NodeStack.Push(new HuffmanNode(left, right));
                }
            }

            HuffmanNode tree = NodeStack.Pop(); // there should be just one node left

            Dictionary<string, char> EncodingCharacters = new Dictionary<string, char>(); // maps encodings to character
            GenerateEncodings(tree, EncodingCharacters, "");

            string currentEncoding = ""; // string, not int array, as arrays/lists compared by instance, not contents

            while (true)
            {
                int nextBit = byteReader.Read();
                if (nextBit == -1) break;

                currentEncoding += nextBit.ToString();

                if (!EncodingCharacters.ContainsKey(currentEncoding)) continue;

                rawFile += EncodingCharacters[currentEncoding];

                currentEncoding = "";
            }
        }

        try
        {
            File.WriteAllText(path, rawFile);
        }
        catch
        {
            Console.WriteLine("Error: Could not write to " + path);
        }
    }

    private static void GenerateEncodings(HuffmanNode tree, Dictionary<string, char> EncodingCharacters, string current)
    {
        if (tree == null) return;

        GenerateEncodings(tree.GetLeft(), EncodingCharacters, current + "0");
        GenerateEncodings(tree.GetRight(), EncodingCharacters, current + "1");

        if (tree.HasCharacter()) EncodingCharacters[current] = tree.GetCharacter();
    }
}
