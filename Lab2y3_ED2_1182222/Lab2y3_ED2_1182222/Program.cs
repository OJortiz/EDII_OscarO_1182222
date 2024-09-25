using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; //Necesaria para trabajar con archivos JSON
using System.Diagnostics;
using Lab2y3_ED2_1182222;

public class Program
{
    public static void Main(string[] args)
    {
        int equalCount = 0; //Variables para conteo de compresiones
        int decompressCount = 0;
        int huffmanCount = 0;
        int arithmeticCount = 0;
        int eitherCount = 0; 

        Inventory inventory = new Inventory(); //Nuevo inventario
        //Ruta de los archivos, de libros, busquedas y resultados
        string inputLogFilePath = @"100Klab01_books.csv";
        string searchFilePath = @"100Klab01_search.csv";
        string outputFilePath = @"search_results.txt";

        var log = File.ReadAllLines(inputLogFilePath); //Entrada de libros
        Stopwatch stopwatch = new Stopwatch(); //Reloj para medir tiempo de ejecución
        stopwatch.Start();

        foreach (var entry in log) //Recorrer todos los libros
        {
            var parts = entry.Split(';'); //Separar si encuentra un punto y coma
            var operation = parts[0].Trim(); //Obtener la operacion INSERT, PATCH o DELETE
            var json = parts[1].Trim();// Contenido del JSON

            if (operation == "INSERT")
            {
                var book = JsonConvert.DeserializeObject<Book>(json);
                inventory.InsertBook(book); //Insertar libros
            }
            else if (operation == "PATCH")
            {
                var updateData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json); //clave con dato a actualizar
                var isbn = updateData["isbn"]; //Obtener isbn a actualizar
                updateData.Remove("isbn");
                //Variables para actualizar
                string name = updateData.ContainsKey("name") ? updateData["name"] : null;
                string author = updateData.ContainsKey("author") ? updateData["author"] : null;
                string category = updateData.ContainsKey("category") ? updateData["category"] : null;
                decimal? price = updateData.ContainsKey("price") ? decimal.Parse(updateData["price"]) : (decimal?)null;
                int? quantity = updateData.ContainsKey("quantity") ? int.Parse(updateData["quantity"]) : (int?)null;

                inventory.UpdateBook(isbn, name, author, category, price, quantity); //Actualizar libros
            }
            else if (operation == "DELETE")
            {
                var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var isbn = deleteData["isbn"]; //Eliminar libro según ISBN
                inventory.DeleteBook(isbn);
            }
        }

        var searchQueries = File.ReadAllLines(searchFilePath); //Obtener todas las búsquedas
        List<string> searchResults = new List<string>();

        foreach (var entry in searchQueries) //Recorrer todas las búsquedas
        {
            var parts = entry.Split(';');
            if (parts[0].Trim() == "SEARCH") 
            {
                var json = parts[1].Trim();
                var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var name = searchData["name"];

                var results = inventory.SearchBooksByName(name);
                foreach (var book in results) //recorrer el resultado 
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
                        equalCount++; //Original = Huffman = Aritmetico
                    }
                    else if ((originalNameSizeBits < nameSizeHuffman) && (originalNameSizeBits < nameSizeArithmeticBits))
                    {
                        decompressCount++; //Original < huffman, original < Aritmetico
                    }
                    else if ((originalNameSizeBits > nameSizeHuffman) && (nameSizeHuffman < nameSizeArithmeticBits))
                    {
                        huffmanCount++; //huffman < original < aritmetico
                    }
                    else if ((originalNameSizeBits > nameSizeArithmeticBits) && (nameSizeArithmeticBits < nameSizeHuffman))
                    {
                        arithmeticCount++; //aritmetico < original < huffman
                    }
                    else if (nameSizeHuffman == nameSizeArithmeticBits)
                    {
                        eitherCount++; // Caso para Huffman == Aritmética
                    }

                    searchResults.Add(JsonConvert.SerializeObject(resultJson));
                }
            }
        }

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
        Console.WriteLine($"Tiempo de TOTAL de ejecución: {stopwatch.ElapsedMilliseconds} ms"); //Tiempo total
        Console.WriteLine($"Resultados guardados en {outputFilePath}"); //Nombre del archivo con resultado
    }

}
