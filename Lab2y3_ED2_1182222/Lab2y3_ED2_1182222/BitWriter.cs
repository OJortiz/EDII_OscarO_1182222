using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    // Clase para escribir bits individuales en un MemoryStream
    public class BitWriter : IDisposable
    {
        private MemoryStream _memoryStream; 
        private byte _currentByte; 
        private int _bitPosition; 
        private bool _disposed = false; 

        // Constructor que inicializa el MemoryStream y establece el byte actual y la posición del bit
        public BitWriter()
        {
            _memoryStream = new MemoryStream();
            _currentByte = 0;
            _bitPosition = 7; // Empieza desde el bit más significativo
        }

        // Método para escribir un solo bit en el MemoryStream
        public void WriteBit(bool bit)
        {
            // Si el bit es verdadero, se establece en la posición correspondiente del byte actual
            if (bit)
            {
                _currentByte |= (byte)(1 << _bitPosition);
            }
            _bitPosition--; 

            // Si se han escrito todos los bits en el byte, lo escribe en el MemoryStream
            if (_bitPosition < 0)
            {
                _memoryStream.WriteByte(_currentByte); // Escribe el byte completo
                _currentByte = 0; 
                _bitPosition = 7; 
            }
        }

        // Método para finalizar la escritura de bits y obtener el MemoryStream con los bits escritos
        public MemoryStream Flush()
        {
            // Si quedan bits sin escribir (menos de un byte completo), escribe el byte restante
            if (_bitPosition < 7)
            {
                _memoryStream.WriteByte(_currentByte);
            }
            return _memoryStream;
        }

        // Método para liberar los recursos (MemoryStream)
        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this); 
        }

        // Método protegido para implementar el patrón Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _memoryStream.Dispose(); 
                }
                _disposed = true; 
            }
        }
    }
}
