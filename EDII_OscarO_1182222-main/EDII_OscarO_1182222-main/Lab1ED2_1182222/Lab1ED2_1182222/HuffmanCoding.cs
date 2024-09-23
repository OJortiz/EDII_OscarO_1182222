using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1ED2_1182222
{
    public class HuffmanCoding
    {
        public static Dictionary<char, int> CalculateFrequencies(string text)
        {
            var frequencies = new Dictionary<char, int>();
            foreach (var character in text)
            {
                if (frequencies.ContainsKey(character))
                    frequencies[character]++;
                else
                    frequencies[character] = 1;
            }
            return frequencies;
        }

        public static (string encoded, int sizeInBits) Encode(string text)
        {
            var frequencies = CalculateFrequencies(text);
            var huffmanTree = new HuffmanTree(frequencies);
            var codes = huffmanTree.GenerateCodes();

            var encoded = string.Concat(text.Select(c => codes[c]));
            int sizeInBits = encoded.Length;

            return (encoded, sizeInBits);
        }
    }

}
