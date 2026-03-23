int fails = 0;

for (int i = 0; i < 500; i++)
{
    System.Numerics.BigInteger e = 65537;

    System.Numerics.BigInteger p = RSA.GenerateRSAPrime(e);
    System.Numerics.BigInteger q = RSA.GenerateRSAPrime(e);
    System.Numerics.BigInteger n = p*q;

    System.Numerics.BigInteger phin = (p-1)*(q-1);

    System.Numerics.BigInteger d = RSA.GetModMultInverse(e, phin);


    Org.BouncyCastle.Math.BigInteger bouncyCastlePhin = new Org.BouncyCastle.Math.BigInteger(1, phin.ToByteArray(true, true));
    Org.BouncyCastle.Math.BigInteger bouncyCastleE    = new Org.BouncyCastle.Math.BigInteger(1,    e.ToByteArray(true, true));
    Org.BouncyCastle.Math.BigInteger bouncyCastleD    = new Org.BouncyCastle.Math.BigInteger(1,    d.ToByteArray(true, true));

    if (bouncyCastleE.ModInverse(bouncyCastlePhin).Equals(bouncyCastleD))
    {
        Console.WriteLine($"Pass: ({n}, {e}, {d}) is a valid key");
    }
    else
    {
        Console.WriteLine($"FAIL: ({n}, {e}, {d}) is NOT a valid key");
        fails++;
    }
}

Console.WriteLine("Failed " + fails + " times.");
