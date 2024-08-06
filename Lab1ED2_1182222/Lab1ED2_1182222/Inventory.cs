using System.Collections.Generic;
using System.Linq;

namespace Lab1ED2_1182222
{
    public class Inventory
    {
        private List<Book> books; //Lista de libros

        public Inventory() //Constructor del inventario
        {
            books = new List<Book>();
        }

        public void InsertBook(Book book) //Recibe un libro y lo agrega a la lista
        {
            books.Add(book);
        }

        public void DeleteBook(string isbn) //Recibe el ISBN y borra un libro
        {
            var book = books.FirstOrDefault(b => b.ISBN == isbn); //Busca el primer libro en el que el ISBN enviado coincida con el ISBN de un libro en la lista
            if (book != null) 
            {
                books.Remove(book); //Si el libro es distinto a null, lo borra
            }
        }

        public void UpdateBook(string isbn, string author = null, decimal? price = null, int? quantity = null)
        {
            var book = books.FirstOrDefault(b => b.ISBN == isbn); //Busca coincidencias entre los ISBN
            if (book != null) //Si no es null
            {
                if (author != null) book.Author = author; //Los datos se actualizan a los enviados en el parametro
                if (price.HasValue) book.Price = price.Value;
                if (quantity.HasValue) book.Quantity = quantity.Value;
            }
        }

        public List<Book> SearchBooksByName(string name) //Funcion de busqueda de libros
        {
            return books.Where(b => b.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)).ToList(); //Busca las coincidencias en los nombres y los devuelve como lista
        }
    }
}
