private static bool MillerRabin(BigInteger n, int rounds)
{
    int s = 1;
    while ((n-1) % (BigInteger)Math.Pow(2, s+1) == 0) s++;

    BigInteger d = (n-1)/(BigInteger)(Math.Pow(2,s));

    for (int i = 0; i < rounds; i++)
    {
        BigInteger a = GenerateInRange(2, n-3);

        BigInteger x = BigInteger.ModPow(a, d, n);

        for (int j = 0; j < s; j++)
        {
            BigInteger y = BigInteger.ModPow(x, 2, n);

            if (y == 1 && x != 1 && x != n-1) return false;

            x = y;
        }

        if (x != 1) return false;
    }

    return true;
}
