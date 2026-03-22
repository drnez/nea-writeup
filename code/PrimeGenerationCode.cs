using System.Security.Cryptography;
using System.Numerics;

static class RSA
{
    public static BigInteger GenerateRSAPrime(BigInteger e) // e must be prime in this implementation!
    {
        while (true)
        {
            BigInteger bigInt;

            do bigInt = GenerateOdd512BitInt();
            while (bigInt % e == 1); // ensures e will be relatively prime with phi(n), as e used is prime

            if (MillerRabin(bigInt, 40)) return bigInt;
        }
    }

    private static BigInteger GenerateOdd512BitInt()
    {
        byte[] randomInt = new byte[64]; // 512 bits = 64 bytes
        RandomNumberGenerator.Fill(randomInt);

        randomInt[0] |= (byte)(128); // sets MSB to 1, so num is 512 bits

        randomInt[63] |= (byte)(1); // sets LSB to 1 - odd

        return new BigInteger(randomInt, true, true); // isUnsigned, isBigEndian
    }

    private static BigInteger GenerateInRange(BigInteger lower, BigInteger upper)
    {
        BigInteger num = -1;

        while (num < lower || num > upper)
        {
            byte[] randomInt = new byte[64]; // 512 bits = 64 bytes
            RandomNumberGenerator.Fill(randomInt);

            num = new BigInteger(randomInt, true, true);
        }

        return num;
    }

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
}
