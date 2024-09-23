using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1ED2_1182222
{
    public static class StringExtension
    {
        public static string Byte(this string binaryString)
        {
            // Divide the combined binary string into 4-bit groups separated by hyphens
            StringBuilder formattedBinaryStringBuilder = new();
            for (int i = 0; i < binaryString.Length; i += 4)
            {
                if (i > 0)
                {
                    formattedBinaryStringBuilder.Append('-');
                }

                // Append each 4-bit group
                if (i + 4 <= binaryString.Length)
                    formattedBinaryStringBuilder.Append(binaryString.AsSpan(i, 4));
                else
                    formattedBinaryStringBuilder.Append(binaryString.AsSpan(i));

            }
            return formattedBinaryStringBuilder.ToString();
        }
    }
}
