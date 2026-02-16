private static BigInteger GetModMultInverse(BigInteger a, BigInteger m) // uses the extended euclidian algorithm to find x s.t. ax = 1 (mod m)
{
    BigInteger r_0 = a;
    BigInteger s_0 = 1;
    BigInteger t_0 = 0;

    BigInteger r_1 = m;
    BigInteger s_1 = 0;
    BigInteger t_1 = 1;

    BigInteger r_2, s_2, t_2;

    do
    {
        BigInteger q = r_0 / r_1;

        r_2 = r_0 - q * r_1;
        s_2 = s_0 - q * s_1;
        t_2 = t_0 - q * t_1;

        (r_0, r_1) = (r_1, r_2);
        (s_0, s_1) = (s_1, s_2);
        (t_0, t_1) = (t_1, t_2);

    } while (r_2 != 0);

    BigInteger d = s_0 % m;
    if (d < 0) d += m;

    return d;
}
