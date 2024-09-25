using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    // Clase para leer bits individuales desde un MemoryStream
    public class BitReader : IDisposable
    {
        private readonly MemoryStream _memoryStream; // MemoryStream desde donde se leen los bits
        private byte _currentByte; 
        private int _bitIndex; 
        private bool _disposed; 

        // Constructor que inicializa el BitReader con un MemoryStream
        public BitReader(MemoryStream memoryStream)
        {
            _memoryStream = memoryStream ?? throw new ArgumentNullException(nameof(memoryStream));
            _bitIndex = 8; // Comienza al principio del primer byte
            memoryStream.Position = 0; // Asegura que la posición en el MemoryStream sea al inicio
        }

        // Método para leer un solo bit
        public bool ReadBit()
        {
            if (_disposed) // Verifica si el objeto ya ha sido liberado
                throw new ObjectDisposedException(nameof(BitReader));

            // Si ya se han leído todos los bits del byte actual, lee un nuevo byte
            if (_bitIndex == 8)
            {
                // Si no hay más bytes en el stream, retorna false
                if (_memoryStream.Position >= _memoryStream.Length)
                    return false;

                // Lee el siguiente byte del stream
                _currentByte = (byte)_memoryStream.ReadByte();
                _bitIndex = 0; // Reinicia el índice de bits para el nuevo byte
            }

            // Obtiene el bit en la posición actual y aumenta el índice
            bool bit = (_currentByte & 1 << (7 - _bitIndex)) != 0;
            _bitIndex++;
            return bit;
        }

        // Método para liberar recursos, implementa el patrón Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Libera los recursos gestionados, en este caso el MemoryStream
                    _memoryStream?.Dispose();
                }

                _disposed = true; 
            }
        }

        // Implementación de la interfaz IDisposable
        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this); // Evita que el recolector de basura llame al finalizador
        }

        // Finalizador que se llama si no se llama a Dispose explícitamente
        ~BitReader()
        {
            Dispose(false); 
        }
    }
}
