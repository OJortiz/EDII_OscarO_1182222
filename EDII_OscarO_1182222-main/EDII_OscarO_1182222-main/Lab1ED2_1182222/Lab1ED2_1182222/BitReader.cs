using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1ED2_1182222
{
    public class BitReader : IDisposable
    {
        private readonly MemoryStream _memoryStream;
        private byte _currentByte;
        private int _bitIndex;
        private bool _disposed;

        public BitReader(MemoryStream memoryStream)
        {
            _memoryStream = memoryStream ?? throw new ArgumentNullException(nameof(memoryStream));
            _bitIndex = 8; // Start at the beginning of the first byte
            memoryStream.Position = 0;
        }

        public bool ReadBit()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BitReader));

            if (_bitIndex == 8) // Load new byte if the current byte is exhausted
            {
                if (_memoryStream.Position >= _memoryStream.Length)
                    return false;

                _currentByte = (byte)_memoryStream.ReadByte();
                _bitIndex = 0; // Reset bit index for the new byte
            }

            bool bit = (_currentByte & 1 << 7 - _bitIndex) != 0;
            _bitIndex++;
            return bit;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if needed
                    _memoryStream?.Dispose();
                }

                // Free any unmanaged resources here if needed

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BitReader()
        {
            Dispose(false);
        }
    }
}
