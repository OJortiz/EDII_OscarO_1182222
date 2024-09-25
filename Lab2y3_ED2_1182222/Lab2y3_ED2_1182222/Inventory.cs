using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Lab2y3_ED2_1182222;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    public class Inventory
    {
        private Dictionary<string, Book> isbnDict; // Diccionario con clave ISBN
        private Dictionary<string, Book> nameDict; // Diccionario con clave name

        public Inventory() //Constructor del inventario
        {
            isbnDict = new Dictionary<string, Book>();
            nameDict = new Dictionary<string, Book>(StringComparer.OrdinalIgnoreCase);
        }

        public void InsertBook(Book book) 
        {

            if (!isbnDict.ContainsKey(book.ISBN))   // Verifica si el libro con ese ISBN no está ya en el diccionario
            {
                // Si el ISBN no existe, inserta el libro tanto en el diccionario de ISBN como en el de nombres
                isbnDict[book.ISBN] = book;
                nameDict[book.Name] = book;
            }
        }

        // Método para eliminar un libro basado en su ISBN
        public void DeleteBook(string isbn)
        {
            // Verifica si el libro con ese ISBN está presente en el diccionario
            if (isbnDict.TryGetValue(isbn, out var book))
            {
                // Si se encuentra, se elimina tanto del diccionario de ISBN como del diccionario de nombres
                isbnDict.Remove(isbn);
                nameDict.Remove(book.Name);
            }
        }

        // Método para actualizar la información de un libro basado en su ISBN
        public void UpdateBook(string isbn, string name = null, string author = null, string category = null, decimal? price = null, int? quantity = null)
        {
            // Verifica si el libro con ese ISBN está presente en el diccionario
            if (isbnDict.TryGetValue(isbn, out var book))
            {
                // Si se proporciona un nuevo nombre y es diferente del actual, actualiza el nombre
                if (name != null && book.Name != name)
                {
                    nameDict.Remove(book.Name);
                    book.Name = name;
                    nameDict[book.Name] = book;
                }
                // Actualiza los demás campos si se proporcionan nuevos valores
                if (author != null) book.Author = author;
                if (category != null) book.Category = category;
                if (price.HasValue) book.Price = price.Value;
                if (quantity.HasValue) book.Quantity = quantity.Value;
            }

        }

        // Método para buscar libros por nombre
        public List<Book> SearchBooksByName(string name)
        {

            var results = new List<Book>();
            // Busca el libro en el diccionario de nombres
            if (nameDict.TryGetValue(name, out var book))
            {
                results.Add(book);
            }

            return results; // Retorna la lista de resultados (puede ser vacía si no se encontró)
        }
    }
}
