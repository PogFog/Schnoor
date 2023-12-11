using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chu_ky_Schnorr
{
    public class Khoacongkhai
    {
        public BigInteger p { get; set; }
        public BigInteger q { get; set; }
        public BigInteger y { get; set; }
        public BigInteger g { get; set; }
        public Khoacongkhai(BigInteger p, BigInteger q, BigInteger g, BigInteger y)
        {
            this.p = p;
            this.q = q;
            this.g = g;
            this.y = y;
        }
    }
}
