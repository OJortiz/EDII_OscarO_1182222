using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2y3_ED2_1182222
{
    public class Book
    {
        //atributos de los libros
        [JsonProperty("isbn")]
        public string ISBN { get; set; }
        [JsonProperty("name")]//Se requiere colocar JsonProperty para que imprima los resultados entre comillas
        public string Name { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("category")] // Nuevo campo para categoría
        public string Category { get; set; }

        [JsonIgnore]
        public decimal Price { get; set; }
        [JsonIgnore]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public string PriceString => Price.ToString();//Convierte los valores numericos a string para ponerlos entre comillas
        [JsonProperty("quantity")]
        public string QuantityString => Quantity.ToString();

        public Book(string isbn, string name, string author, string category, decimal price, int quantity) //Constructor de los libros
        {
            ISBN = isbn;
            Name = name;
            Author = author;
            Category = category; // Asignación de categoría
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this); // Retorno en formato JSON, incluyendo la categoría
        }
    }
}
