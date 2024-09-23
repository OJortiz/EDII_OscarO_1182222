using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1ED2_1182222
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public int CompareTo(HuffmanNode other)
        {
            if (this.Frequency == other.Frequency)
            {
                return this.Symbol.CompareTo(other.Symbol); // Ordenar por símbolo si la frecuencia es igual
            }
            return this.Frequency.CompareTo(other.Frequency);
        }
    }

}
