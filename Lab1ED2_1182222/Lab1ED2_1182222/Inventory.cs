using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lab1ED2_1182222
{
    public class Inventory
    {
        private Dictionary<string, Book> isbnDict; // For ISBN-based operations
        private Dictionary<string, Book> nameDict; // For name-based searches

        public Inventory()
        {
            isbnDict = new Dictionary<string, Book>();
            nameDict = new Dictionary<string, Book>(StringComparer.OrdinalIgnoreCase);
        }

        public void InsertBook(Book book)
        {

            if (!isbnDict.ContainsKey(book.ISBN))
            {
                isbnDict[book.ISBN] = book;
                nameDict[book.Name] = book;
            }
        }

        public void DeleteBook(string isbn)
        {

            if (isbnDict.TryGetValue(isbn, out var book))
            {
                isbnDict.Remove(isbn);
                nameDict.Remove(book.Name);
            }
        }

        public void UpdateBook(string isbn, string name = null, string author = null, string category = null, decimal? price = null, int? quantity = null)
        {

            if (isbnDict.TryGetValue(isbn, out var book))
            {
                // If the name is updated, remove the old name from the nameDict
                if (name != null && book.Name != name)
                {
                    nameDict.Remove(book.Name);
                    book.Name = name;
                    nameDict[book.Name] = book;
                }
                if (author != null) book.Author = author;
                if (category != null) book.Category = category;
                if (price.HasValue) book.Price = price.Value;
                if (quantity.HasValue) book.Quantity = quantity.Value;
            }

        }

        public List<Book> SearchBooksByName(string name)
        {

            var results = new List<Book>();
            if (nameDict.TryGetValue(name, out var book))
            {
                results.Add(book);
            }

            return results;
        }
    }
}