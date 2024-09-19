
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Lab1ED2_1182222;

public class Program
{
    public static void Main(string[] args)
    {
        Inventory inventory = new Inventory();

        string inputLogFilePath = @"100Klab01_books.csv";
        string searchFilePath = @"100Klab01_search.csv";
        string outputFilePath = @"search_results.txt";

        var log = File.ReadAllLines(inputLogFilePath);

        foreach (var entry in log)
        {
            var parts = entry.Split(';');
            var operation = parts[0].Trim();
            var json = parts[1].Trim();

            if (operation == "INSERT")
            {
                var book = JsonConvert.DeserializeObject<Book>(json);
                inventory.InsertBook(book);
            }
            else if (operation == "PATCH")
            {
                var updateData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var isbn = updateData["isbn"];
                updateData.Remove("isbn");

                string name = updateData.ContainsKey("name") ? updateData["name"] : null; // Verifica si el nombre está en los datos para actualizar
                string author = updateData.ContainsKey("author") ? updateData["author"] : null;
                string category = updateData.ContainsKey("category") ? updateData["category"] : null; // Manejo de categoría
                decimal? price = updateData.ContainsKey("price") ? decimal.Parse(updateData["price"]) : (decimal?)null;
                int? quantity = updateData.ContainsKey("quantity") ? int.Parse(updateData["quantity"]) : (int?)null;

                inventory.UpdateBook(isbn, name, author, category, price, quantity); // Actualización de categoría
            }
            else if (operation == "DELETE")
            {
                var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var isbn = deleteData["isbn"];
                inventory.DeleteBook(isbn);
            }
        }

        var searchQueries = File.ReadAllLines(searchFilePath);
        List<string> searchResults = new List<string>();

        foreach (var entry in searchQueries)
        {
            var parts = entry.Split(';');
            if (parts[0].Trim() == "SEARCH")
            {
                var json = parts[1].Trim();
                var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var name = searchData["name"];

                var results = inventory.SearchBooksByName(name);
                foreach (var book in results)
                {
                    searchResults.Add(book.ToString());
                }
            }
        }

        File.WriteAllLines(outputFilePath, searchResults);

        Console.WriteLine($"Resultados guardados en {outputFilePath}");
    }
}