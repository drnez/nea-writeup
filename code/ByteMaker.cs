class ByteMaker
{
    private int[] _bits;
    private int _bitIndex;
    private int _trailingZeros;

    private List<byte> _bytes;

    public ByteMaker(bool isCompressed)
    {
        _bits = [isCompressed ? 1 : 0, 0, 0, 0, 0, 0, 0, 0]; // length 8
        // first bit indicates compressed, next 3 bits for trailing zeros
        _bitIndex = 4; 
        _trailingZeros = 0;

        _bytes = new List<byte>();
    }

    public void Add(int bit)
    {
        if (bit != 1 && bit != 0) return;

        _bits[_bitIndex++] = bit;
        
        if (_bitIndex == 8) updateByte();
    }

    public void Add(List<int> bits)
    {
        foreach (int bit in bits) Add(bit);
    }

    public void Add(char character)
    {
        byte characterByte = (byte)character;

        for (int i = 7; i >= 0; i--)
        {
            // right-shift, then AND with 1 to get "last" bit
            int bit = (characterByte >> i) & 1;

            Add(bit);
        }
    }

    public byte[] Export()
    {
        updateByte();

        // push to bits 1-4, OR since already 0
        _bytes[0] |= (byte)(_trailingZeros << 4);
        
        return _bytes.ToArray();
    }
        
    private void updateByte()
    {
        byte newByte = new byte();

        for (int i = 0; i < 8; i++)
        {
            // left-shift, then OR with existing byte
            newByte |= (byte)(_bits[i] << (7-i));
        }

        _bytes.Add(newByte);

        _bits = [0, 0, 0, 0, 0, 0, 0, 0]; // length 8
        _trailingZeros = 8 - _bitIndex;
        _bitIndex = 0;
    }
}
