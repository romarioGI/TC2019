using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Codes
{
    public class Lz78WithHuffmanCoding : Lz78Coding
    {
        private readonly HuffmanСoding _huffmanСoding;

        public Lz78WithHuffmanCoding()
        {
            _huffmanСoding = new HuffmanСoding();
        }

        private string ListOfPairsToString(List<Pair<char, int>> list)
        {
            var res = new StringBuilder();
            foreach (var p in list)
            {
                res.Append(p.Second);
                if(char.IsDigit(p.First))
                    res.Append('♫');
                res.Append(p.First);
            }

            return res.ToString();
        }

        public override BitArray Code(string text, out int codeLength)
        {
            var list = GetListOfPairs(text);
            var newText = ListOfPairsToString(list);

            return _huffmanСoding.Code(newText, out codeLength);
        }

        private List<Pair<char, int>> StringToListOfPairs(string text)
        {
            var res = new List<Pair<char, int>>();
            for (var i = 0; i < text.Length; i++)
            {
                var num = 0;
                while (char.IsDigit(text[i]))
                {
                    num *= 10;
                    num += text[i++] - '0';
                }

                if (text[i] == '♫')
                    i++;

                var symbol = text[i];

                res.Add(new Pair<char, int>(symbol, num));
            }

            return res;
        }

        public override string Decode(BitArray code, out int codeLength)
        {
            var newText = _huffmanСoding.Decode(code, out codeLength);
            var codeList = StringToListOfPairs(newText);

            return GetText(codeList);
        }
    }
}
