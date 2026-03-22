int fails = 0;

for (int i = 0; i < 500; i++)
{
    Org.BouncyCastle.Math.BigInteger p = new Org.BouncyCastle.Math.BigInteger(1, RSA.GenerateRSAPrime(65537).ToByteArray(true, true));

    if (p.IsProbablePrime(100))
    {
        Console.WriteLine("Pass: " + p + " is prime");
    }
    else
    {
        Console.WriteLine("FAIL: " + p + " is NOT prime");
        fails++;
    }
}

Console.WriteLine("Failed " + fails + " times.");
