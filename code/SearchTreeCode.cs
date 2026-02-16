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
