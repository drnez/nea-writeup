// place characters in priorityqueue (based on freq) to generate the tree
PriorityQueue<HuffmanNode, int> NodeQueue = new PriorityQueue<HuffmanNode, int>(); // "base" trees of size 1

foreach (KeyValuePair<char, int> kvp in CharacterCounts)
{
    NodeQueue.Enqueue(new HuffmanNode(kvp.Key, kvp.Value), kvp.Value);
}

HuffmanNode tree = MergeNodes(NodeQueue); // tree = top node of tree

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
