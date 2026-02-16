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
