using System.Collections;
using System.Collections.Generic;

namespace Codes
{
    public static class CodesDictionaryCoder
    {
        public static BitArray GetCode(Dictionary<char, BitArray> codesDictionary)
        {
            var bitArrays = new List<BitArray>();
            var cnt = BitArrayConverter.ConvertToBitArray(codesDictionary.Count, 8);
            bitArrays.Add(cnt);

            foreach (var pair in codesDictionary)
            {
                var symbol = BitArrayConverter.ConvertToBitArray(pair.Key, 16);
                var length = BitArrayConverter.ConvertToBitArray(pair.Value.Length, 5);
                var code = pair.Value;

                bitArrays.Add(symbol);
                bitArrays.Add(length);
                bitArrays.Add(code);
            }

            return BitArrayConverter.ConvertToBitArray(bitArrays);
        }

        public static Dictionary<BitArray, char> GetDictionary(BitArray code, ref int index)
        {
            var res = new Dictionary<BitArray, char>(new BitArrayComparer());
            var cnt = BitArrayConverter.ConvertToInt(code, ref index, 8);

            for (var _ = 0; _ < cnt; _++)
            {
                var symbol = BitArrayConverter.ConvertToInt(code, ref index, 16);
                var length = BitArrayConverter.ConvertToInt(code, ref index, 5);
                var newCode = new BitArray(length);

                for (var i = 0; i < length; i++)
                {
                    newCode[i] = code[index];
                    index++;
                }

                res.Add(newCode, (char)symbol);
            }

            return res;
        }
    }
}
