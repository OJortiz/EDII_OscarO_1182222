using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lab1ED2_1182222
{
    public class Inventory
    {
        private List<Book> books;

        public Inventory()
        {
            books = new List<Book>();
        }

        public void InsertBook(Book book)
        {
            books.Add(book);
        }

        public void DeleteBook(string isbn)
        {
            var book = books.FirstOrDefault(b => b.ISBN == isbn);
            if (book != null)
            {
                books.Remove(book);
            }
        }

        public void UpdateBook(string isbn, string name = null, string author = null, string category = null, decimal? price = null, int? quantity = null)
        {
            var book = books.FirstOrDefault(b => b.ISBN == isbn);
            if (book != null)
            {
                if (name != null) book.Name = name;  // Añadimos la lógica para actualizar el nombre
                if (author != null) book.Author = author;
                if (category != null) book.Category = category; // Actualiza la categoría
                if (price.HasValue) book.Price = price.Value;
                if (quantity.HasValue) book.Quantity = quantity.Value;
            }
        }

        public List<Book> SearchBooksByName(string name)
        {
            return books.Where(b => b.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}