using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char Symbol { get; set; } // Símbolo asociado al nodo
        public int Frequency { get; set; } // Frecuencia de aparición del símbolo
        public HuffmanNode Left { get; set; } // Hijo izquierdo (subárbol izquierdo)
        public HuffmanNode Right { get; set; } // Hijo derecho (subárbol derecho)

        // Compara los nodos en base a su frecuencia y, si son iguales, por el símbolo
        public int CompareTo(HuffmanNode other)
        {
            if (this.Frequency == other.Frequency)
            {
                return this.Symbol.CompareTo(other.Symbol);
            }
            return this.Frequency.CompareTo(other.Frequency);
        }
    }
}

