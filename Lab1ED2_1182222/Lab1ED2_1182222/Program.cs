using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; //Importante para trabajar con JSON 
using Lab1ED2_1182222;

public class Program
{
    public static void Main(string[] args)
    {
        Inventory inventory = new Inventory(); //Nuevo inventario

        // Ruta de los archivos CSV
        string inputLogFilePath = @"lab01_books.csv";
        string searchFilePath = @"lab01_search.csv";
        string outputFilePath = @"search_results.txt";

        // Leer bitácora de entrada desde un archivo CSV
        var log = File.ReadAllLines(inputLogFilePath);

        foreach (var entry in log) //Recorrer todo el archivo de libros
        {
            var parts = entry.Split(';'); //Dividir en cada punto y coma
            var operation = parts[0].Trim(); //Elimina los espacios en blanco y asigna operaciones
            var json = parts[1].Trim();//Asigna los valores del JSON

            if (operation == "INSERT")
            {
                var book = JsonConvert.DeserializeObject<Book>(json);//Toma los datos de la variable json y la convierte en una instancia de book
                inventory.InsertBook(book);//Inserta el libro
            }
            else if (operation == "PATCH")
            {
                var updateData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); //Guarda la información a actualizar con una llave
                var isbn = updateData["isbn"]; //Guarda la llave
                updateData.Remove("isbn");

                string author = updateData.ContainsKey("author") ? updateData["author"] : null; //Evalua si los atributos son null
                decimal? price = updateData.ContainsKey("price") ? decimal.Parse(updateData["price"]) : (decimal?)null;
                int? quantity = updateData.ContainsKey("quantity") ? int.Parse(updateData["quantity"]) : (int?)null;

                inventory.UpdateBook(isbn, author, price, quantity); //Actualiza los datos del libro, con los nuevos valores
            }
            else if (operation == "DELETE")
            {
                var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); //Guarda los datos a borrar con una llave
                var isbn = deleteData["isbn"]; //Guarda la llave
                inventory.DeleteBook(isbn); //Borra el libro según su llave
            }
        }

        // Leer archivo de búsqueda y buscar libros por nombre
        var searchQueries = File.ReadAllLines(searchFilePath);
        List<string> searchResults = new List<string>();

        foreach (var entry in searchQueries) //Recorre todas las lineas de SEARCH
        {
            var parts = entry.Split(';'); //Divide al encontrar punto y coma
            if (parts[0].Trim() == "SEARCH") //Si la primera parte es de busqueda 
            {
                var json = parts[1].Trim(); //Elimina espacios en el json del libro
                var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); //Guarda la informacion a buscar en un diccionario
                var name = searchData["name"];

                var results = inventory.SearchBooksByName(name); //Trae cada resultado que coincida los nombres con los libros en inventario
                foreach (var book in results)
                {
                    searchResults.Add(book.ToString());
                }
            }
        }

        // Escribir los resultados en el archivo de salida
        File.WriteAllLines(outputFilePath, searchResults);

        Console.WriteLine($"Resultados guardados en {outputFilePath}");
    }
}
