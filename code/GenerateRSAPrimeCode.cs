private static BigInteger GenerateRSAPrime(BigInteger e) // e must be prime in this implementation!
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
