using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWorm
{
    public class Book
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Category { get; set; }

        public Book(string author, string title, string year, string category = "Без категории")
        {
            Author = author;
            Title = title;
            Year = year;
            Category = string.IsNullOrWhiteSpace(category) ? "Без категории" : category;
        }
    }
}