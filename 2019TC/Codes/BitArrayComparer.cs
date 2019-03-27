using System;
using System.Collections;
using System.Collections.Generic;

namespace Codes
{
    internal class BitArrayComparer : IEqualityComparer<BitArray>
    {
        public bool Equals(BitArray x, BitArray y)
        {
            if(x == null || y == null)
                throw new ArgumentNullException();

            if (x.Length != y.Length)
                return false;
            for (var i = 0; i < x.Length; i++)
                if (x[i] != y[i])
                    return false;
            return true;
        }

        public int GetHashCode(BitArray obj)
        {
            var res = 0;
            foreach (var b in obj)
            {
                res *= 3;
                res++;
                if ((bool)b)
                    res++;
            }

            res ^= obj.Length;

            return res;
        }
    }

}
