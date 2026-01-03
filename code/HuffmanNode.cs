class HuffmanNode
{
    private char _character;
    private int _frequency;
    private HuffmanNode _left;
    private HuffmanNode _right;

    private bool _hasCharacter; // "isLeaf"

    public HuffmanNode(char character, int frequency = 1)
    {
        _character = character;
        _frequency = frequency;

        _hasCharacter = true;
    }

    public HuffmanNode(HuffmanNode left, HuffmanNode right, int frequency = 1)
    {
        _left = left;
        _right = right;
        _frequency = frequency;

        _hasCharacter = false;
    }

    public int GetFrequency() => _frequency;

    public HuffmanNode GetLeft() => _left;
    public HuffmanNode GetRight() => _right;

    public bool HasCharacter() => _hasCharacter;
    public char GetCharacter() => _character;
}

// NOTE: tree is just a "top-node" - links to left/right
