using System.Collections;
using System.Collections.Generic;

namespace Codes
{
    public static class FrequencyDictionaryCoder
    {
        public static BitArray GetCode(Dictionary<char, int> frequencyDictionary)
        {
            var bitArrays = new List<BitArray>();
            var cnt = BitArrayConverter.ConvertToBitArray(frequencyDictionary.Count, 8);
            bitArrays.Add(cnt);

            foreach (var pair in frequencyDictionary)
            {
                var symbol = BitArrayConverter.ConvertToBitArray(pair.Key, 16);
                var code = BitArrayConverter.ConvertToBitArray(pair.Value, 32);

                bitArrays.Add(symbol);
                bitArrays.Add(code);
            }

            return BitArrayConverter.ConvertToBitArray(bitArrays);
        }

        public static Dictionary<char, int> GetDictionary(BitArray code, ref int index)
        {
            var res = new Dictionary<char, int>();
            var cnt = BitArrayConverter.ConvertToInt(code, ref index, 8);

            for (var _ = 0; _ < cnt; _++)
            {
                var symbol = BitArrayConverter.ConvertToInt(code, ref index, 16);
                var newCode = BitArrayConverter.ConvertToInt(code, ref index, 32);

                res.Add((char) symbol, newCode);
            }

            return res;
        }
    }
}
