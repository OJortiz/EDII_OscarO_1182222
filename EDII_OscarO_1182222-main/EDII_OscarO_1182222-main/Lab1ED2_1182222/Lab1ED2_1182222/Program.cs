using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Lab1ED2_1182222;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        int equalCount = 0;
        int decompressCount = 0;
        int huffmanCount = 0;
        int arithmeticCount = 0;
        int eitherCount = 0; // Nueva variable para Huffman == Aritmética

        Inventory inventory = new Inventory();

        string inputLogFilePath = @"100Klab01_books.csv";
        string searchFilePath = @"100Klab01_search.csv";
        string outputFilePath = @"search_results2.txt";

        var log = File.ReadAllLines(inputLogFilePath);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

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
                    // Codificación Huffman
                    var (encodedNameHuffman, nameSizeHuffman) = HuffmanCoding.Encode(book.Name);

                    var originalNameSizeBits = book.Name.Length * 16; // 2 bytes por carácter = 16 bits

                    // Codificación Aritmética (dinámica)
                    var arithmeticCoder = new ArithmeticCompressionInt();
                    var encodedNameArithmetic = arithmeticCoder.Compress(book.Name);
                    var nameSizeArithmeticBits = encodedNameArithmetic.Length * 8; // Convertir de bytes a bits

                    var resultJson = new
                    {
                        isbn = book.ISBN,
                        name = book.Name,
                        author = book.Author,
                        category = book.Category,
                        price = book.PriceString,
                        quantity = book.QuantityString,
                        namesize = book.Name.Length * 2, // En bytes
                        namesizehuffman = nameSizeHuffman,
                        namesizearithmetic = encodedNameArithmetic.Length // En bytes para el resultado
                    };

                    // Comparación con tamaños en bits
                    if ((originalNameSizeBits == nameSizeHuffman) && (originalNameSizeBits == nameSizeArithmeticBits))
                    {
                        equalCount++;
                    }
                    else if ((originalNameSizeBits < nameSizeHuffman) && (originalNameSizeBits < nameSizeArithmeticBits))
                    {
                        decompressCount++;
                    }
                    else if ((originalNameSizeBits > nameSizeHuffman) && (nameSizeHuffman < nameSizeArithmeticBits))
                    {
                        huffmanCount++;
                    }
                    else if ((originalNameSizeBits > nameSizeArithmeticBits) && (nameSizeArithmeticBits < nameSizeHuffman))
                    {
                        arithmeticCount++;
                    }
                    else if (nameSizeHuffman == nameSizeArithmeticBits)
                    {
                        eitherCount++; // Caso para Huffman == Aritmética
                    }

                    searchResults.Add(JsonConvert.SerializeObject(resultJson));
                }
            }
        }

        // Imprimir resumen final
        Console.WriteLine($"Equal: {equalCount}");
        Console.WriteLine($"Decompress: {decompressCount}");
        Console.WriteLine($"Huffman: {huffmanCount}");
        Console.WriteLine($"Arithmetic: {arithmeticCount}");
        Console.WriteLine($"Either: {eitherCount}");

        // Guardar resultados en el archivo de salida
        File.WriteAllLines(outputFilePath, searchResults);

        // Agregar resumen al final del archivo
        using (StreamWriter writer = File.AppendText(outputFilePath))
        {
            writer.WriteLine($"Equal: {equalCount}");
            writer.WriteLine($"Decompress: {decompressCount}");
            writer.WriteLine($"Huffman: {huffmanCount}");
            writer.WriteLine($"Arithmetic: {arithmeticCount}");
            writer.WriteLine($"Either: {eitherCount}");
        }

        stopwatch.Stop();
        Console.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Resultados guardados en {outputFilePath}");
    }

}
