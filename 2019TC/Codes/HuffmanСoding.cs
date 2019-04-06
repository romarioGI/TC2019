using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Codes
{
    public class HuffmanСoding : ICode
    {
        private class Node : IComparable<Node>
        {
            private readonly Node _left;
            private readonly Node _right;
            private readonly int _p;

            public BitArray Code { get; private set; }
            public char Symbol { get; }

            public Node(Node left, Node right)
            {
                _left = left;
                _right = right;
                _p = _left._p + _right._p;
                Symbol = _left.Symbol;
            }

            public Node(int frequency, char symbol)
            {
                _p = frequency;
                Symbol = symbol;
            }

            private void BuildCodes(BitArray code)
            {
                if (_left == null && _right == null)
                {
                    Code = new BitArray(code);
                    return;
                }

                if (_left != null)
                {
                    code.Length++;
                    code[code.Length - 1] = false;
                    _left.BuildCodes(code);
                    code.Length--;
                }

                if (_right != null)
                {
                    code.Length++;
                    code[code.Length - 1] = true;
                    _right.BuildCodes(code);
                    code.Length--;
                }
            }

            public int CompareTo(Node other)
            {
                if (_p != other._p)
                    return _p.CompareTo(other._p);
                return Symbol.CompareTo(other.Symbol);
            }

            private void GetCodesDictionary(Dictionary<char, BitArray> codesDictionary)
            {
                if (Code != null)
                {
                    codesDictionary.Add(Symbol, Code);
                    return;
                }

                if (_left != null)
                    _left.GetCodesDictionary(codesDictionary);
                if (_right != null)
                    _right.GetCodesDictionary(codesDictionary);
            }

            public Dictionary<char, BitArray> GetCodesDictionary()
            {
                var res = new Dictionary<char, BitArray>();
                BuildCodes(new BitArray(0));
                GetCodesDictionary(res);

                return res;
            }
        }

        private Node BuildTree(Dictionary<char, int> frequencies)
        {
            var nodes = new SortedSet<Node>();
            foreach (var pair in frequencies)
            {
                var curNode = new Node(pair.Value, pair.Key);
                nodes.Add(curNode);
            }

            while (nodes.Count > 1)
            {
                var left = nodes.Min;
                nodes.Remove(left);
                var right = nodes.Min;
                nodes.Remove(right);
                nodes.Add(new Node(left, right));
            }

            return nodes.Min;
        }

        private Dictionary<char, BitArray> GetCodesDictionary(Dictionary<char, int> frequency)
        {
            var root = BuildTree(frequency);
            return root.GetCodesDictionary();
        }

        private BitArray TextCode(string text, Dictionary<char, BitArray> codesDictionary)
        {
            var bitArrays = new List<BitArray>();

            foreach (var c in text)
                bitArrays.Add(codesDictionary[c]);

            return BitArrayConverter.ConvertToBitArray(bitArrays);
        }

        private BitArray GetCode(string text, Dictionary<char, BitArray> codesDictionary, out int codeLength)
        {
            var dictionaryCode = CodesDictionaryCoder.GetCode(codesDictionary);
            var textCode = TextCode(text, codesDictionary);
            codeLength = textCode.Length;

            return BitArrayConverter.ConvertToBitArray(new List<BitArray>() {dictionaryCode, textCode});
        }

        public BitArray Code(string text, out int codeLength)
        {
            var frequencyDictionary = FrequencyDictionaryMaker.Get(text);
            var codesDictionary = GetCodesDictionary(frequencyDictionary);
            return GetCode(text, codesDictionary, out codeLength);
        }

        private string TextDecode(BitArray code, int index, Dictionary<BitArray, char> decodesDictionary)
        {
            var res = new StringBuilder();
            var cur = new BitArray(0);
            while (index < code.Length)
            {
                cur.Length++;
                cur[cur.Length - 1] = code[index];
                index++;
                if (decodesDictionary.ContainsKey(cur) == false)
                    continue;
                res.Append(decodesDictionary[cur]);
                cur.Length = 0;
            }

            return res.ToString();
        }

        public string Decode(BitArray code, out int codeLength)
        {
            var index = 0;
            var dict = CodesDictionaryCoder.GetDictionary(code, ref index);
            codeLength = code.Length - index;
            return TextDecode(code, index, dict);
        }
    }
}
