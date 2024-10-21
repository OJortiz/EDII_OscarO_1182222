using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Lab1ED2_1182222;
using Lab04EDII_1182222;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        Inventory inventory = new Inventory();

        string inputLogFilePath = @"100Klab01_books.csv";
        string searchFilePath = @"100Klab01_search.csv";
        string outputFilePath = @"search_results.txt";
        string encryptedFilePathManualDES = @"search_results_manual_DES.txt.enc";
        string encryptedFilePathCSharpDES = @"search_results_csharp_DES.txt.enc";
        string decryptedFilePathManualDES = @"search_results_manual_DES_decrypted.txt";
        string decryptedFilePathCSharpDES = @"search_results_csharp_DES_decrypted.txt";

        var log = File.ReadAllLines(inputLogFilePath);

        // Procesar los datos del archivo CSV
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

                string name = updateData.ContainsKey("name") ? updateData["name"] : null;
                string author = updateData.ContainsKey("author") ? updateData["author"] : null;
                string category = updateData.ContainsKey("category") ? updateData["category"] : null;
                decimal? price = updateData.ContainsKey("price") ? decimal.Parse(updateData["price"]) : (decimal?)null;
                int? quantity = updateData.ContainsKey("quantity") ? int.Parse(updateData["quantity"]) : (int?)null;

                inventory.UpdateBook(isbn, name, author, category, price, quantity);
            }
            else if (operation == "DELETE")
            {
                var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var isbn = deleteData["isbn"];
                inventory.DeleteBook(isbn);
            }
        }

        // Procesar las consultas de búsqueda
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

        // Guardar los resultados de la búsqueda
        File.WriteAllLines(outputFilePath, searchResults);
        Console.WriteLine($"Resultados guardados en {outputFilePath}");

        string opcion = string.Empty;
        while (true)
        {
            Console.WriteLine("\nElige el método de encriptación:");
            Console.WriteLine("1. Encriptación manual (DES implementado)");
            Console.WriteLine("2. Encriptación con bibliotecas de C# (DESEncryption)");
            Console.WriteLine("3. Salir del programa");
            opcion = Console.ReadLine();

            if (opcion == "1" || opcion == "2" || opcion == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Opción no válida. Por favor ingresa 1, 2 o 3.");
            }
        }

        if (opcion == "3")
        {
            Console.WriteLine("Saliendo del programa...");
            return;
        }

        string desencriptar = string.Empty;
        while (true)
        {
            Console.WriteLine("\n¿Quieres también desencriptar el archivo después de encriptar?");
            Console.WriteLine("1. Sí");
            Console.WriteLine("2. No, solo encriptar");
            desencriptar = Console.ReadLine();

            if (desencriptar == "1" || desencriptar == "2")
            {
                break;
            }
            else
            {
                Console.WriteLine("Opción no válida. Por favor ingresa 1 o 2.");
            }
        }

        if (opcion == "1")
        {
            string key = "ok:uo1IN"; 
            var byteskey = Encoding.ASCII.GetBytes(key);

            DES.EncryptFile(outputFilePath, encryptedFilePathManualDES, byteskey);
            Console.WriteLine($"Archivo encriptado con DES manual: {encryptedFilePathManualDES}");

            if (desencriptar == "1")
            {
                DES.DecryptFile(encryptedFilePathManualDES, decryptedFilePathManualDES, byteskey);
                Console.WriteLine($"Archivo desencriptado con DES manual: {decryptedFilePathManualDES}");
            }
        }
        else if (opcion == "2")
        {
            DESEncryption.EncryptFile(outputFilePath, encryptedFilePathCSharpDES);
            Console.WriteLine($"Archivo encriptado con C# DES: {encryptedFilePathCSharpDES}");

            if (desencriptar == "1")
            {
                DESEncryption.DecryptFile(encryptedFilePathCSharpDES, decryptedFilePathCSharpDES);
                Console.WriteLine($"Archivo desencriptado con C# DES: {decryptedFilePathCSharpDES}");
            }
        }
    }
}
