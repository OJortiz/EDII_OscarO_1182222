using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1ED2_1182222
{
    public class HuffmanTree
    {
        private readonly Dictionary<char, int> frequencies;
        private readonly PriorityQueue<HuffmanNode, int> priorityQueue; // Cambiado aquí

        public HuffmanTree(Dictionary<char, int> frequencies)
        {
            this.frequencies = frequencies;
            priorityQueue = new PriorityQueue<HuffmanNode, int>(); // Cambiado aquí

            BuildTree();
        }

        private void BuildTree()
        {
            foreach (var symbol in frequencies)
            {
                priorityQueue.Enqueue(new HuffmanNode { Symbol = symbol.Key, Frequency = symbol.Value }, symbol.Value); // Cambiado aquí
            }

            while (priorityQueue.Count > 1)
            {
                var left = priorityQueue.Dequeue();
                var right = priorityQueue.Dequeue();
                var merged = new HuffmanNode
                {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };
                priorityQueue.Enqueue(merged, merged.Frequency); // Cambiado aquí
            }
        }

        public Dictionary<char, string> GenerateCodes()
        {
            var codes = new Dictionary<char, string>();
            GenerateCodes(priorityQueue.Peek(), "", codes);
            return codes;
        }

        private void GenerateCodes(HuffmanNode node, string code, Dictionary<char, string> codes)
        {
            if (node.Left == null && node.Right == null)
            {
                codes[node.Symbol] = code;
                return;
            }

            if (node.Left != null)
                GenerateCodes(node.Left, code + "0", codes);
            if (node.Right != null)
                GenerateCodes(node.Right, code + "1", codes);
        }
    }


}
