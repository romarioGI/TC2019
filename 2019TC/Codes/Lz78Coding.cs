using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Codes
{
    public class Lz78Coding : ICode
    {
        private class LzNode
        {
            public readonly int NodeNumber;
            private readonly Dictionary<char, LzNode> _nextLzNodes;

            public LzNode(int nodeNumber)
            {
                NodeNumber = nodeNumber;
                _nextLzNodes = new Dictionary<char, LzNode>();
            }

            public bool ContainsNextSymbol(char symbol)
            {
                return _nextLzNodes.ContainsKey(symbol);
            }

            public void Add(char symbol, LzNode node)
            {
                _nextLzNodes.Add(symbol, node);
            }

            public LzNode this[char symbol]
            {
                get { return _nextLzNodes[symbol]; }
            }
        }

        protected struct Pair<T1, T2>
        {
            public T1 First;
            public T2 Second;

            public Pair(T1 first, T2 second)
            {
                First = first;
                Second = second;
            }
        }

        private const int LengthOfNumber = 17;
        private const int DictionaryCapacity = 1 << LengthOfNumber;

        protected List<Pair<char, int>> GetListOfPairs(string text)
        {
            var res = new List<Pair<char, int>>();
            var root = new LzNode(0);
            var curNode = root;
            var index = 0;
            var nodeCnt = 1;
            while (index < text.Length)
            {
                if (curNode.ContainsNextSymbol(text[index]))
                {
                    curNode = curNode[text[index]];
                    index++;
                    continue;
                }

                res.Add(new Pair<char, int>(text[index], curNode.NodeNumber));

                if (nodeCnt < DictionaryCapacity)
                {
                    curNode.Add(text[index], new LzNode(nodeCnt));
                    nodeCnt++;
                }

                index++;
                curNode = root;
            }
            res.Add(new Pair<char, int>('\0', curNode.NodeNumber));

            return res;
        }

        private BitArray ListOfPairsToBitArray(List<Pair<char, int>> listOfPairs)
        {
            var arrays = new List<BitArray>();
            foreach (var pair in listOfPairs)
            {
                arrays.Add(BitArrayConverter.ConvertToBitArray(pair.First));
                arrays.Add(BitArrayConverter.ConvertToBitArray(pair.Second,LengthOfNumber));
            }

            return BitArrayConverter.ConvertToBitArray(arrays);
        }

        public virtual BitArray Code(string text, out int codeLength)
        {
            var list = GetListOfPairs(text);
            var code = ListOfPairsToBitArray(list);
            codeLength = code.Length;

            return code;
        }

        private List<Pair<char, int>> BitArrayToListOfPairs(BitArray code)
        {
            var res = new List<Pair<char, int>>();
            var index = 0;
            while (index < code.Length)
            {
                var symbol = BitArrayConverter.ConvertToChar(code, ref index);
                var number = BitArrayConverter.ConvertToInt(code, ref index, LengthOfNumber);
                res.Add(new Pair<char, int>(symbol, number));
            }

            return res;
        }

        protected string GetText(List<Pair<char, int>> codeList)
        {
            var res = new StringBuilder();
            
            var pairsList = new List<Pair<char, int>>();
            foreach (var p in codeList)
            {
                var word = new Stack<char>();
                pairsList.Add(p);
                var curPair = p;
                while (curPair.Second != 0)
                {
                    word.Push(curPair.First);
                    curPair = pairsList[curPair.Second - 1];
                }

                word.Push(curPair.First);
                foreach (var c in word)
                    res.Append(c);
            }

            return res.ToString();
        }

        public virtual string Decode(BitArray code, out int codeLength)
        {
            codeLength = code.Length;
            var listOfPairs = BitArrayToListOfPairs(code);
            return GetText(listOfPairs);
        }
    }
}
