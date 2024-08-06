using Newtonsoft.Json;
using System;

namespace Lab1ED2_1182222
{
    public class Book
    {
        [JsonProperty("isbn")]//atributos de los libros
        public string ISBN { get; set; }
        [JsonProperty("name")] //Se requiere colocar JsonProperty para que imprima los resultados entre comillas
        public string Name { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonIgnore]
        public decimal Price { get; set; }
        [JsonIgnore]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public string PriceString => Price.ToString(); //Convierte los valores numericos a string para ponerlos entre comillas
        [JsonProperty("quantity")]
        public string QuantityString => Quantity.ToString();

        public Book(string isbn, string name, string author, decimal price, int quantity) //Constructor de los libros
        {
            ISBN = isbn;
            Name = name;
            Author = author;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this); //Retorno de los libros buscados en formato jsonLine
        }
    }
}
