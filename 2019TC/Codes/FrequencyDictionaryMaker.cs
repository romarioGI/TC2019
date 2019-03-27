using System.Collections.Generic;

namespace Codes
{
    internal static class FrequencyDictionaryMaker
    {
        public static Dictionary<char, int> Get(string text)
        {
            var res = new Dictionary<char, int>();
            foreach (var c in text)
                if (res.ContainsKey(c))
                    res[c]++;
                else
                    res.Add(c, 1);

            return res;
        }
    }
}
