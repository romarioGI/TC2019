using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Codes
{
    internal class RationalNumber
    {
        private static BigInteger Gcd(BigInteger first, BigInteger second)
        {
            while (second !=0)
            {
                var item = first % second;
                first = second;
                second = item;
            }

            return first;
        }

        private readonly BigInteger _numerator;
        private readonly BigInteger _denominator;

        public RationalNumber(int x, int y):this((BigInteger)x,y)
        {
        }

        private RationalNumber(BigInteger x, BigInteger y)
        {
            if (y == 0)
                throw new ArgumentOutOfRangeException();
            _numerator = x;
            _denominator = y;
            var d = Gcd(x, y);
            if (d != 1)
            {
                _numerator /= d;
                _denominator /= d;
            }

            if (_denominator < 0)
            {
                _denominator *= -1;
                _numerator *= -1;
            }
        }

        public int FirstDigitAfterDot()
        {
            return (int) (10 * _numerator / _denominator);
        }

        public static RationalNumber operator +(RationalNumber first, RationalNumber second)
        {
            return new RationalNumber(first._numerator * second._denominator + first._denominator * second._numerator,
                first._denominator * second._denominator);
        }

        public static RationalNumber operator *(RationalNumber first, RationalNumber second)
        {
            return new RationalNumber(first._numerator*second._numerator, second._denominator*first._denominator);
        }

        public static RationalNumber operator /(RationalNumber first, RationalNumber second)
        {
            return first * new RationalNumber(second._denominator, second._numerator);
        }

        public static bool operator <(RationalNumber first, RationalNumber second)
        {
            var c = second - first;
            return c._numerator > 0;
        }

        public static bool operator >(RationalNumber first, RationalNumber second)
        {
            var c = second - first;
            return c._numerator < 0;
        }

        public static implicit operator RationalNumber(int x)
        {
            return new RationalNumber(x, 1);
        }

        public static implicit operator RationalNumber(BigInteger x)
        {
            return new RationalNumber(x, 1);
        }

        public static RationalNumber operator -(RationalNumber first, RationalNumber second)
        {
            return first + (-1) * second;
        }
    }

    public class ArithmeticCoding:ICode
    {
        private bool RetrieveCode(ref RationalNumber a, ref RationalNumber b, ref BigInteger code, ref int digitCnt)
        {
            var digit = a.FirstDigitAfterDot();
            if (digit != b.FirstDigitAfterDot())
                return false;
            a = 10 * a - digit;
            b = 10 * b - digit;
            code = 10 * code + digit;
            digitCnt++;

            return true;
        }

        private BitArray TextCode(string text, Dictionary<char, int> frequencyDictionary)
        {
            var res = new BigInteger(0);
            var digitCnt = 0;

            var probabilities = new List<RationalNumber>();
            var mapSymbolsInNumber = new Dictionary<char, int>();
            probabilities.Add(0);
            foreach (var pair in frequencyDictionary)
            {
                mapSymbolsInNumber.Add(pair.Key, probabilities.Count);
                probabilities.Add(new RationalNumber(pair.Value, text.Length) + probabilities[probabilities.Count - 1]);
            }

            RationalNumber a = 0;
            RationalNumber b = 1;

            foreach (var c in text)
            {
                var index = mapSymbolsInNumber[c];
                var length = b - a;
                b = probabilities[index] * length + a;
                a = probabilities[index - 1] * length + a;
                while (RetrieveCode(ref a, ref b, ref res, ref digitCnt))
                {
                }
            }

            res = res * 10 + a.FirstDigitAfterDot()+1;
            digitCnt++;

            return BitArrayConverter.ConvertToBitArray(res, digitCnt);
        }

        private BitArray GetCode(string text, Dictionary<char, int> frequencyDictionary,out int codeLength)
        {
            var length = BitArrayConverter.ConvertToBitArray(text.Length, 32);
            var dictionaryCode = FrequencyDictionaryCoder.GetCode(frequencyDictionary);
            var textCode = TextCode(text, frequencyDictionary);
            codeLength = textCode.Length;

            return BitArrayConverter.ConvertToBitArray(new List<BitArray>() {length, dictionaryCode, textCode});
        }

        public BitArray Code(string text, out int codeLength)
        {
            var frequencyDictionary = FrequencyDictionaryMaker.Get(text);
            return GetCode(text, frequencyDictionary, out codeLength);
        }

        private int LowerBound(List<RationalNumber> list, RationalNumber x)
        {
            int l = 0, r = list.Count;
            while (r - l > 1)
            {
                var m = (r + l) / 2;
                if (list[m] < x)
                    l = m;
                else
                    r = m;
            }

            return r;
        }

        private string TextDecode(BitArray code, int index, Dictionary<char, int> frequencyDictionary, int textLength)
        {
            var res = new StringBuilder();
            var probabilities = new List<RationalNumber> {0};
            var mapNumberInSymbols = new Dictionary<int,char>();
            foreach (var pair in frequencyDictionary)
            {
                mapNumberInSymbols.Add(probabilities.Count, pair.Key);
                probabilities.Add(new RationalNumber(pair.Value, textLength) + probabilities[probabilities.Count - 1]);              
            }

            RationalNumber a = 0;
            RationalNumber b = 1;
            var c = (RationalNumber)BitArrayConverter.ConvertToBigInteger(code, ref index, code.Length - index, out var digitCnt);
            c /= BigInteger.Pow(10, digitCnt);

            for (var i = 0; i < textLength; i++)
            {
                var length = b - a;
                var ind = LowerBound(probabilities, (c - a) / length);
                res.Append(mapNumberInSymbols[ind]);
                b = probabilities[ind] * length + a;
                a = probabilities[ind - 1] * length + a;
            }

            return res.ToString();
        }

        public string Decode(BitArray code, out int codeLength)
        {
            var index = 0;
            var length = BitArrayConverter.ConvertToInt(code, ref index, 32);
            var dict = FrequencyDictionaryCoder.GetDictionary(code, ref index);
            codeLength = code.Length - index;
            return TextDecode(code, index, dict, length);
        }
    }
}
