static class HuffmanEncoder
{
    public static byte[] WriteCompressedFile(string text)
    {
        HuffmanNode tree = GenerateTree(text);

        // add encoding of tree
        ByteMaker byteMaker = new ByteMaker(true);

        Dictionary<char, List<int>> CharacterEncodings = new Dictionary<char, List<int>>();

        SearchTree(tree, byteMaker, CharacterEncodings, new List<int>());
        byteMaker.Add(0);
        
        // add encoding of text
        foreach (char c in text) byteMaker.Add(CharacterEncodings[c]);

        byte[] bytes = byteMaker.Export();

        if (bytes.Length > text.Length)
        {
            ByteMaker textByteMaker = new ByteMaker(false);
            
            foreach(char c in text) textByteMaker.Add(c);

            bytes = textByteMaker.Export();
        }

        return bytes;
    }

    private static HuffmanNode GenerateTree(string text)
    {
        // link characters to their count
        Dictionary<char, int> CharacterCounts = new Dictionary<char, int>();

        foreach (char c in text)
        {
            if (CharacterCounts.ContainsKey(c)) CharacterCounts[c]++;
            else CharacterCounts[c] = 1;
        }

        // place characters in priorityqueue (based on freq) to generate the tree
        PriorityQueue<HuffmanNode, int> NodeQueue = new PriorityQueue<HuffmanNode, int>(); // "base" trees of size 1

        foreach (KeyValuePair<char, int> kvp in CharacterCounts)
        {
            NodeQueue.Enqueue(new HuffmanNode(kvp.Key, kvp.Value), kvp.Value);
        }

        HuffmanNode tree = MergeNodes(NodeQueue); // tree = top node of tree

        return tree;
    }

    // recursively merge nodes to make a tree
    private static HuffmanNode MergeNodes(PriorityQueue<HuffmanNode, int> trees)
    {
        if (trees.Count == 1) return trees.Dequeue();

        HuffmanNode tree1 = trees.Dequeue();
        HuffmanNode tree2 = trees.Dequeue();

        int newFreq = tree1.GetFrequency() + tree2.GetFrequency();

        HuffmanNode newTree = new HuffmanNode(tree1, tree2, newFreq);

        trees.Enqueue(newTree, newFreq);

        return MergeNodes(trees);
    }

    // fill byteMaker with encoded tree, and CharacterEncodings with bits encoding characters, uses post order DFS
    private static void SearchTree(HuffmanNode tree, ByteMaker byteMaker, Dictionary<char, List<int>> CharacterEncodings, List<int> current)
    {
        if (tree == null) return;

        SearchTree(tree.GetLeft(), byteMaker, CharacterEncodings, current.Append(0).ToList());
        SearchTree(tree.GetRight(), byteMaker,  CharacterEncodings, current.Append(1).ToList());

        if (tree.HasCharacter()) // is leaf
        {
            byteMaker.Add(1);
            byteMaker.Add(tree.GetCharacter());

            CharacterEncodings[tree.GetCharacter()] = current;
        }
        else
        {
            byteMaker.Add(0);
        }
    }
}
