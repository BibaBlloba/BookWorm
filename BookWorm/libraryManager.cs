using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BookWorm
{
    public class LibraryManager
    {
        public List<Book> Books { get; private set; }
        public LibraryManager()
        {
            Books = new List<Book>();
            LoadBooks();
        }
        public void AddBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            Books.Add(book);
            SaveBooks();
        }
        public void RemoveBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            Books.Remove(book);
            SaveBooks();
        }
        public List<Book> SearchBooks(string query)
        {
            return Books.Where(b => b.Author.Contains(query) || b.Title.Contains(query)).ToList();
        }
        private void SaveBooks()
        {
            File.WriteAllLines("books.txt", Books.Select(b => $"{b.Author}|{b.Title}|{b.Year}"));
        }
        private void LoadBooks()
        {
            if (File.Exists("books.txt"))
            {
                var lines = File.ReadAllLines("books.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        Books.Add(new Book(parts[0], parts[1], parts[2]));
                    }
                }
            }
        }
    }
}