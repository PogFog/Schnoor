using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Chu_ky_Schnorr
{
    internal class ToanHoc
    {
        private static Random random = new Random(DateTime.Now.Second);
        static public BigInteger GenerateLargePrime(int bitSize)
        {
            Random rand = new Random();
            // Kích thước bit của số nguyên tố

            BigInteger prime = BigInteger.Zero;
            while (!IsPrime(prime))
            {
                byte[] bytes = new byte[bitSize / 8];
                rand.NextBytes(bytes);
                prime = new BigInteger(bytes);
                prime = BigInteger.Abs(prime);
            }

            return prime;
        }
        // Hàm sinh số nguyên tố rời rạc
        static public BigInteger GenerateDiscreteLogPrime(BigInteger p)
        {
            Random rand = new Random();

            while (true)
            {
                BigInteger q = GenerateRandomBigInteger(2, p - 1);

                if (p % q == 1)
                    continue;

                if (IsPrime(q))
                    return q;
            }
        }

        // Hàm sinh số nguyên g là generator
        static public BigInteger GenerateGenerator(BigInteger p, BigInteger q)
        {
            Random rand = new Random();

            while (true)
            {
                BigInteger g = GenerateRandomBigInteger(2, p - 1);
                BigInteger power = (p - 1) / q;
                BigInteger result = BigInteger.ModPow(g, power, p);

                if (result != 1)
                    return g;
            }
        }

        // Hàm sinh số nguyên ngẫu nhiên trong khoảng từ min đến max
        static public BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
        {
            Random rand = new Random();

            BigInteger result = BigInteger.Zero;

            while (result < min || result > max)
            {
                byte[] bytes = new byte[max.ToByteArray().Length];
                rand.NextBytes(bytes);
                result = new BigInteger(bytes);
            }

            return result;
        }

        // Hàm băm thông điệp
        static public BigInteger Hash(BigInteger r, string message)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
                byte[] rBytes = r.ToByteArray();

                byte[] combinedBytes = new byte[messageBytes.Length + rBytes.Length];
                Array.Copy(messageBytes, 0, combinedBytes, 0, messageBytes.Length);
                Array.Copy(rBytes, 0, combinedBytes, messageBytes.Length, rBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(combinedBytes);

                BigInteger hashValue = ConvertToUnsignedBigInteger(hashBytes);
                BigInteger q = BigInteger.One << (hashBytes.Length * 8); // Tính giá trị q tương ứng với độ dài băm

                return BigInteger.ModPow(hashValue, BigInteger.One, q);
            }
        }
        static BigInteger ConvertToUnsignedBigInteger(byte[] bytes)
        {
            byte[] unsignedBytes = new byte[bytes.Length + 1];
            Array.Copy(bytes, 0, unsignedBytes, 1, bytes.Length);
            Array.Reverse(unsignedBytes);
            return new BigInteger(unsignedBytes);
        }

        public static BigInteger ModuloPower(BigInteger a, BigInteger b, BigInteger c)
        {
            if (b == 0)
                return 1;

            BigInteger temp = ModuloPower(a, b / 2, c) % c;
            BigInteger result = (temp * temp) % c;

            if (b % 2 == 1)
                result = (result * a) % c;

            return ((result + c) % c);
        }
    

        static public BigInteger RandomBigInteger(BigInteger min, BigInteger max)
        {
            if (min > max) return 0;
            int len = max.ToByteArray().Length;
            byte[] buffer = new byte[len];
            random.NextBytes(buffer);
            var tmp = new BigInteger(buffer);
            if (tmp < 0) tmp = -tmp;
            return tmp % (max - min + 1) + min;
        }
        static private BigInteger[] heSo(BigInteger n)
        {
            long s = 0;
            while ((n & 1) == 0)
            {
                s++;
                n >>= 1;
            }
            return new BigInteger[] { s, n };
        }
        static private bool checkMillerRabin(BigInteger s, BigInteger d, BigInteger n, BigInteger a)
        {
            if (n == a) return true;
            var p = BigInteger.ModPow(a, d, n);
            if (p == 1) return true;
            while (s > 0)
            {
                if (p == n - 1) return true;
                p = p * p % n;
                s--;
            }
            return false;
        }
        static public bool IsPrime(BigInteger n)
        {
            if (n < 2) return false;
            if ((n & 1) == 0) return n == 2;
            var heso = heSo(n - 1);
            var s = heso[0];
            var d = heso[1];
            long[] ran = { 2, 3, 5, 7, 23, 11, 17, 61 };
            foreach (long e in ran)
            {
                if (checkMillerRabin(s, d, n, e) == false)
                    return false;
            }
            return true;
        }
        public static BigInteger GeneratePrime(int keySize)
        {
            BigInteger prime = RandomBigInteger(BigInteger.One << keySize - 1, BigInteger.One << keySize);

            if (prime % 2 == 0) prime++;
            while (!IsPrime(prime)) prime += 2;

            return prime;
        }

        static public BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            var mod = m;
            BigInteger y0 = 0, y1 = 1, y = -1;
            while (a > 1)
            {
                var r = m % a;
                if (r == 0) break;
                var q = m / a;
                y = y0 - y1 * q;
                m = a;
                a = r;
                y0 = y1;
                y1 = y;
            }
            if (a > 1) return -1;
            return (y + mod) % mod;
        }
    }
}
