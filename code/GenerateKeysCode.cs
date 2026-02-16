public static Tuple<BigInteger, BigInteger, BigInteger> GenerateKeys()
{
    BigInteger e = 65537;

    BigInteger p = RSA.GenerateRSAPrime(e);
    BigInteger q = RSA.GenerateRSAPrime(e);

    BigInteger n = p*q;
    BigInteger phin = (p-1)*(q-1);

    BigInteger d = RSA.GetModMultInverse(e, phin);

    return new Tuple<BigInteger, BigInteger, BigInteger>(n,e,d);
}
