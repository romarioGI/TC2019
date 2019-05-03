using System;

namespace HammingCode
{
    static class Coder
    {
        public const int SyndromeLength = 3;
        public const int TextLength = 4;
        public const int CodeLength = 7;

        private static readonly int[,] CodingArray = new[,]
            {{1, 0, 0, 0}, {0, 1, 0, 0}, {0, 0, 1, 0}, {0, 0, 0, 1}, {0, 1, 1, 1}, {1, 0, 1, 1}, {1, 1, 0, 1}};

        private static readonly int[,] DecodingArray = new[,]
            {{0, 0, 0, 1, 1, 1, 1}, {0, 1, 1, 0, 0, 1, 1}, {1, 0, 1, 0, 1, 0, 1}};

        public static string Code(string text)
        {
            if (text.Length > TextLength)
                throw new ArgumentException();

            var textBits = new int[TextLength];
            for (var i = 0; i < TextLength; i++)
                if (text[i] == '0')
                    textBits[i] = 0;
                else if (text[i] == '1')
                    textBits[i] = 1;
                else
                    throw new ArgumentException();

            var code = new int[CodeLength];
            for (var i = 0; i < CodeLength; i++)
                for (var j = 0; j < TextLength; j++)
                    code[i] ^= textBits[j] & CodingArray[i, j];

            return string.Concat(code);
        }

        public static string CorrectError(string code)
        {
            if (code.Length > CodeLength)
                throw new ArgumentException();

            var codeBits = new int[CodeLength];
            for (var i = 0; i < CodeLength; i++)
                if (code[i] == '0')
                    codeBits[i] = 0;
                else if (code[i] == '1')
                    codeBits[i] = 1;
                else
                    throw new ArgumentException();

            var syndrome = new int[SyndromeLength];
            for (var i = 0; i < SyndromeLength; i++)
            for (var j = 0; j < CodeLength; j++)
                syndrome[i] ^= codeBits[j] & DecodingArray[i, j];

            var errorPosition = 0;
            for (var i = 0; i < SyndromeLength; i++)
            {
                errorPosition <<= 1;
                errorPosition += syndrome[i];
            }
            errorPosition--;
            if (errorPosition != -1)
                codeBits[errorPosition] ^= 1;

            return string.Concat(codeBits);

        }

        public static string Decode(string code)
        {
            return code.Substring(0, TextLength);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Введите строку из '0' и '1' длины " + Coder.TextLength);
            var text = Console.ReadLine();
            var code = Coder.Code(text);
            Console.WriteLine("Код этой строки: " + code);
            Console.WriteLine("Введите номер позиции, в которой произошла ошибка, либо 0, если ошибки не было");
            // ReSharper disable once AssignNullToNotNullAttribute
            var errorPosition = int.Parse(Console.ReadLine());
            if (errorPosition != 0)
            {
                errorPosition--;
                var d = code[errorPosition] - '0';
                d ^= 1;
                code = code.Remove(errorPosition, 1).Insert(errorPosition, d.ToString());
                Console.WriteLine("Код с ошибкой: " + code);
                code = Coder.CorrectError(code);
                Console.WriteLine("Исправленный код: " + code);
            }

            text = Coder.Decode(code);
            Console.WriteLine("Раскодированный текст: " + text);
            Console.ReadKey();
        }
    }
}
