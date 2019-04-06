using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Codes
{
    public static class BitArrayConverter
    {
        private static readonly int[] TwoDegrees;

        static BitArrayConverter()
        {
            TwoDegrees = new int[32];
            for (var i = 0; i < 32; i++)
                TwoDegrees[i] = 1 << i;
        }

        public static BitArray ConvertToBitArray(int x, int length)
        {
            var res = new BitArray(length);
            for (var i = 0; i < length; i++)
            {
                res[i] = (x & 1) == 1;
                x >>= 1;
            }

            return res;
        }

        public static BitArray ConvertToBitArray(List<BitArray> bitArrays)
        {
            var length = 0;
            foreach (var a in bitArrays)
                length += a.Length;

            var res = new BitArray(length);
            var index = 0;
            foreach (var a in bitArrays)
            foreach (var b in a)
                res[index++] = (bool)b;

            return res;
        }

        public static BitArray ConvertToBitArray(BigInteger x, int digitCnt)
        {
            var digitCntBits = ConvertToBitArray(digitCnt, 32);
            var res = new BitArray(0);
            do
            {
                res.Length++;
                res[res.Length - 1] = !x.IsEven;
                x /= 2;
            } while (x != 0);

            return ConvertToBitArray(new List<BitArray> {digitCntBits, res});
        }

        public static BitArray ConvertToBitArray(char c)
        {
            return ConvertToBitArray(c, 16);
        }

        public static int ConvertToInt(BitArray bitArray, ref int start, int length)
        {
            var res = 0;
            for (var i = 0; i < length; i++)
            {
                if (bitArray[start])
                    res += TwoDegrees[i];
                start++;
            }

            return res;
        }

        public static char ConvertToChar(BitArray bitArray, ref int start)
        {
            return (char) ConvertToInt(bitArray, ref start, 16);
        }

        public static BigInteger ConvertToBigInteger(BitArray bitArray, ref int start, int length, out int digitCnt)
        {
            var finish = start + length - 1;
            digitCnt = ConvertToInt(bitArray, ref start, 32);
            var res = new BigInteger(0);
            for (var i = finish; i >= start; i--)
            {
                res *= 2;
                if (bitArray[i])
                    res++;
            }

            start += length;

            return res;
        }
    }
}
