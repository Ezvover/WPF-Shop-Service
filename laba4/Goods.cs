using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba4
{
    public class Goods
    {
        public Goods()
        {

        }
        public Goods(string id, string name, string desc, string category, double rate, double price, int amount, string other) 
        {
            Id = id;
            Name = name;
            Desc = desc;
            Category = category;
            Rate = rate;
            Price = price;
            Amount = amount;
            Other = other;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Category { get; set; }
        public double Rate { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string Other { get; set; }
    }
}
