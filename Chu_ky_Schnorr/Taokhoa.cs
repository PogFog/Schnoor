using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chu_ky_Schnorr
{
    public class Taokhoa
    {
        public Khoacongkhai khoaCongKhai;
        public Khoabimat khoaBiMat;
        public Taokhoa(BigInteger p, BigInteger q, BigInteger g, BigInteger x, BigInteger y)
        {
            khoaBiMat = new Khoabimat(x);
            khoaCongKhai = new Khoacongkhai(p, q, g, y);
        }
        static public Taokhoa TaoKhoaNgauNhien()
        {
            BigInteger p = 3;
            BigInteger q = ToanHoc.GenerateLargePrime(512);
            for (int i = 2; i <= 10000; i++)
            {
                if (ToanHoc.IsPrime((q * (i ^ 352)) + 1))
                {
                    p = (q * (i ^ 352)) + 1;
                    break;
                }
                else
                    continue;
            }
            var h = ToanHoc.RandomBigInteger(BigInteger.One, p - 1);
            BigInteger g = BigInteger.ModPow(h, ((p - 1) / q), p);
            var x = ToanHoc.RandomBigInteger(BigInteger.Zero, q);
            var y = BigInteger.ModPow(g, x, p);

            return new Taokhoa(p, q, g, x, y);
        }
        static public Taokhoa TaoKhoaThuCong(BigInteger p, BigInteger q, BigInteger x)
        {
            if (x <= 0 || x > q)
                x = ToanHoc.RandomBigInteger(BigInteger.Zero, q);
            var h = ToanHoc.RandomBigInteger(BigInteger.One, p - 1);
            BigInteger g = BigInteger.ModPow(h, ((p - 1) / q), p);
            var y = BigInteger.ModPow(g, x, p);
            return new Taokhoa(p, q, g, x, y);
        }
    }
}
