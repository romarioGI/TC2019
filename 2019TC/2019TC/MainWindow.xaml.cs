using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using BinaryCodeToImage;
using Codes;
using Microsoft.Win32;
// ReSharper disable PossibleInvalidOperationException

namespace _2019TC
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        private readonly ICode[] _coders = {new HuffmanСoding(), new ArithmeticCoding(), new Lz78Coding(), new Lz78WithHuffmanCoding(), };

        public MainWindow()
        {
            InitializeComponent();
            CodeComboBox.ItemsSource = new[] {"Huffman", "Arithmetic coding", "LZ78", "Lz78 + Huffman" };
            CodeComboBox.SelectedIndex = 0;
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog {Filter = "Image Files(*.BMP)|*.BMP"};
            Bitmap bmp;
            if (ofd.ShowDialog().Value)
            {
                bmp = new Bitmap(ofd.FileName);
            }
            else
            {
                MessageBox.Show("Декодирование прервано");
                return;
            }

            string text;
            int codeLength;

            try
            {
                var code = BitmapCoder.GetCode(bmp);
                text = _coders[CodeComboBox.SelectedIndex].Decode(code, out codeLength);
            }
            catch (Exception c)
            {
                MessageBox.Show("Декодирование прервано. Возможно, картинка закодирована другим алгоритмом");
                return;
            }

            var sfd = new SaveFileDialog {Filter = "Text file(*.txt)|*.txt"};
            if (sfd.ShowDialog().Value)
            {
                var sw = new StreamWriter(sfd.FileName);
                sw.Write(text);
                sw.Close();
            }
            else
            {
                MessageBox.Show("Декодирование прервано");
                return;
            }

            var compressionFactor = (text.Length * 16.0) / codeLength;
            CompressionFactorLabel.Content = "Compression factor = " + compressionFactor;
        }

        private void CodeButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog {Filter = "Text file(*.txt)|*.txt"};
            string text;
            if (ofd.ShowDialog().Value)
            {
                var sr = new StreamReader(ofd.FileName, Encoding.Default);
                text = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                MessageBox.Show("Кодирование прервано");
                return;
            }

            var code = _coders[CodeComboBox.SelectedIndex].Code(text, out var codeLength);

            var sfd = new SaveFileDialog {Filter = "Image Files(*.BMP)|*.BMP"};
            if (sfd.ShowDialog().Value)
            {
                var bmp = BitmapCoder.GetBitmap(code);
                bmp.Save(sfd.FileName);
            }
            else
            {
                MessageBox.Show("Кодирование прервано");
                return;
            }

            var compressionFactor = (text.Length * 16.0) / codeLength;
            CompressionFactorLabel.Content = "Compression factor = " + compressionFactor;
        }
    }
}
