class ByteReader
{
    private List<byte> _bytes;
    private int _bitIndex;
    private int _trailingNum;

    public ByteReader(byte[] bytes, out bool compressed)
    {
        _bytes = bytes.ToList();
        _bitIndex = 0;

        int firstBit = Read();

        if (firstBit == 0) compressed = false;
        else compressed = true;

        int a = Read();
        int b = Read();
        int c = Read();

        _trailingNum = a*4 + b*2 + c;
    }

    public int Read()
    {
        if (_bytes.Count() == 0) return -1;

        // don't read padding zeros
        if (_bytes.Count() == 1 && _bitIndex >= 8 - _trailingNum) return -1;

        int bit = (_bytes[0] >> (7 - _bitIndex++)) & 1;

        Update();

        return bit;
    }

    public char? ReadChar()
    {
        byte nextByte = 0;

        for (int i = 0; i < 8; i++)
        {
            int bit = Read();
            if (bit == -1) return null;

            // shift 1 to its position and OR with exisiting byte
            if (bit == 1) nextByte |= (byte)(1 << (7-i));
        }

        return (char)nextByte;
    }

    private void Update()
    {
        if (_bitIndex != 8) return;

        _bytes.RemoveAt(0);
        _bitIndex = 0;
    }
}
