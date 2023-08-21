using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba4
{
    internal class Categories
    {
        public Categories() 
        {

        }

        public Categories(int id, string category)
        {
            Id = id;
            Category = category;
        }

        public int Id { get; set; }
        public string Category { get; set; }

    }
}
