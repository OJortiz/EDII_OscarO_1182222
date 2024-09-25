using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    public class ArithmeticCompressionInt
    {
        private readonly Dictionary<char, (ushort Low, ushort High)> _probabilities;
        private ulong underflow_bits;
        private const ushort default_low = 0;
        private const ushort default_high = 0xffff;
        private const ushort MSD = 0x8000;
        private const ushort SSD = 0x4000;
        private ushort scale;

        public ArithmeticCompressionInt()
        {
            _probabilities = new Dictionary<char, (ushort Low, ushort High)>();
        }

        private void CalculateProbabilities(string source)
        {
            Dictionary<char, ushort> frequencies = new Dictionary<char, ushort>();

            // Calcular frecuencias de cada carácter
            foreach (char symbol in source)
            {
                if (!frequencies.ContainsKey(symbol))
                    frequencies[symbol] = 0;
                frequencies[symbol]++;
            }

            scale = (ushort)source.Length;
            ushort low = 0;

            // Calcular rangos basados en frecuencias
            foreach (var symbol in frequencies.OrderBy(x => x.Key))
            {
                ushort high = (ushort)(low + symbol.Value);
                _probabilities[symbol.Key] = (low, high);
                low = high;
            }
        }

        public MemoryStream Compress(string input)
        {
            // Calcular probabilidades dinámicas para el nombre del libro
            CalculateProbabilities(input);

            BitWriter _output_stream = new();
            ushort low = default_low;
            ushort high = default_high;
            underflow_bits = 0;
            long range;

            foreach (char symbol in input)
            {
                range = (long)(high - low) + 1;
                high = (ushort)(low + range * _probabilities[symbol].High / scale - 1);
                low = (ushort)(low + range * _probabilities[symbol].Low / scale);

                // Normalización para evitar overflow/underflow
                while (true)
                {
                    if ((high & MSD) == (low & MSD))
                    {
                        _output_stream.WriteBit((high & MSD) != 0);

                        while (underflow_bits > 0)
                        {
                            _output_stream.WriteBit((high & MSD) == 0);
                            underflow_bits--;
                        }
                    }
                    else if ((low & SSD) != 0 && (high & SSD) == 0)
                    {
                        underflow_bits++;
                        low &= 0x3fff;
                        high |= 0x4000;
                    }
                    else
                    {
                        break;
                    }

                    low <<= 1;
                    high <<= 1;
                    high |= 1;
                }
            }

            // Salida final
            _output_stream.WriteBit((low & 0x4000) != 0);
            underflow_bits++;
            while (underflow_bits-- > 0)
                _output_stream.WriteBit((low & 0x4000) == 0);

            return _output_stream.Flush();
        }

        public string Decompress(MemoryStream input, int size)
        {
            // Similar a la compresión, pero usando el flujo de bits para descomprimir
            // Leer los primeros 16 bits de código
            BitReader input_buffer = new(input);
            ushort code = 0;
            for (int i = 0; i < 16; i++)
            {
                code <<= 1;
                code |= (ushort)(input_buffer.ReadBit() ? 1 : 0);
            }

            ushort low = default_low;
            ushort high = default_high;
            string output = string.Empty;

            for (int i = 0; i < size; i++)
            {
                long range = (long)(high - low) + 1;
                ushort scaledValue = (ushort)((((long)(code - low) + 1) * scale - 1) / range);

                char symbol = '\0';
                foreach (var entry in _probabilities)
                {
                    if (scaledValue >= entry.Value.Low && scaledValue < entry.Value.High)
                    {
                        symbol = entry.Key;
                        break;
                    }
                }

                output += symbol;

                high = (ushort)(low + range * _probabilities[symbol].High / scale - 1);
                low = (ushort)(low + range * _probabilities[symbol].Low / scale);

                while (true)
                {
                    if ((high & MSD) == (low & MSD))
                    {
                        code <<= 1;
                        code |= (ushort)(input_buffer.ReadBit() ? 1 : 0);
                    }
                    else if ((low & SSD) != 0 && (high & SSD) == 0)
                    {
                        low &= 0x3fff;
                        high |= 0x4000;
                        code <<= 1;
                        code |= (ushort)(input_buffer.ReadBit() ? 1 : 0);
                    }
                    else
                    {
                        break;
                    }

                    low <<= 1;
                    high <<= 1;
                    high |= 1;
                }
            }

            return output;
        }
    }
}
