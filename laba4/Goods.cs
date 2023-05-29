using System.Windows.Input;

namespace laba4
{
    public class Goods
    {
        public Goods()
        {

        }
        public Goods(int id, string name, string desc, string category, int rate, double price, int amount, string other)
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

        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Category { get; set; }
        public int Rate { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string Other { get; set; }
    }
}
