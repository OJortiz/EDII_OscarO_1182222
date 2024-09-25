using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    public class HuffmanTree
    {
        private readonly Dictionary<char, int> frequencies;
        private readonly PriorityQueue<HuffmanNode, int> priorityQueue; // Cola de prioridad para gestionar los nodos de Huffman

        public HuffmanTree(Dictionary<char, int> frequencies)
        {
            this.frequencies = frequencies;
            priorityQueue = new PriorityQueue<HuffmanNode, int>();

            BuildTree(); // Construcción del árbol de Huffman a partir de las frecuencias
        }

        // Construye el árbol combinando los nodos según sus frecuencias
        private void BuildTree()
        {
            foreach (var symbol in frequencies)
            {
                priorityQueue.Enqueue(new HuffmanNode { Symbol = symbol.Key, Frequency = symbol.Value }, symbol.Value);
            }

            // Combina los nodos hasta que queda solo uno en la cola
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
                priorityQueue.Enqueue(merged, merged.Frequency);
            }
        }

        // Genera los códigos binarios para cada símbolo en base al árbol de Huffman
        public Dictionary<char, string> GenerateCodes()
        {
            var codes = new Dictionary<char, string>();
            GenerateCodes(priorityQueue.Peek(), "", codes);
            return codes;
        }

        // Método recursivo para generar los códigos de cada símbolo
        private void GenerateCodes(HuffmanNode node, string code, Dictionary<char, string> codes)
        {
            if (node.Left == null && node.Right == null) // Si es un nodo hoja, asigna el código
            {
                codes[node.Symbol] = code;
                return;
            }

            if (node.Left != null)
                GenerateCodes(node.Left, code + "0", codes); // Recorrido por la izquierda
            if (node.Right != null)
                GenerateCodes(node.Right, code + "1", codes); // Recorrido por la derecha
        }
    }
}

