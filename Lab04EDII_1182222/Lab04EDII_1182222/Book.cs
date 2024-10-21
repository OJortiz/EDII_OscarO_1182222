using Newtonsoft.Json;
using System;

namespace Lab1ED2_1182222
{
    public class Book
    {
        [JsonProperty("isbn")]
        public string ISBN { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonIgnore]
        public decimal Price { get; set; }
        [JsonIgnore]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public string PriceString => Price.ToString();
        [JsonProperty("quantity")]
        public string QuantityString => Quantity.ToString();

        public Book(string isbn, string name, string author, string category, decimal price, int quantity)
        {
            ISBN = isbn;
            Name = name;
            Author = author;
            Category = category;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}