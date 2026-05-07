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
        private const string BooksFileName = "books.txt";
        private const string CategoriesFileName = "categories.txt";

        public List<Book> Books { get; private set; }
        public List<string> Categories { get; private set; }

        public LibraryManager()
        {
            Books = new List<Book>();
            Categories = new List<string>();
            LoadCategories();
            LoadBooks();
        }

        public void AddCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя категории не может быть пустым.", nameof(name));
            }
            if (Categories.Any(c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Категория с таким именем уже существует.");
            }
            Categories.Add(name);
            SaveCategories();
        }

        public void AddBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            if (string.IsNullOrWhiteSpace(book.Category))
            {
                book.Category = "Без категории";
            }
            if (!Categories.Any(c => string.Equals(c, book.Category, StringComparison.OrdinalIgnoreCase)))
            {
                Categories.Add(book.Category);
                SaveCategories();
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
            return Books.Where(b => b.Author.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    b.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    b.Category.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public List<Book> FilterBooksByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || category == "Все категории")
            {
                return new List<Book>(Books);
            }
            return Books.Where(b => string.Equals(b.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void SaveCategories()
        {
            File.WriteAllLines(CategoriesFileName, Categories);
        }

        private void SaveBooks()
        {
            File.WriteAllLines(BooksFileName, Books.Select(b => $"{b.Author}|{b.Title}|{b.Year}|{b.Category}"));
        }

        private void LoadCategories()
        {
            if (File.Exists(CategoriesFileName))
            {
                var lines = File.ReadAllLines(CategoriesFileName);
                foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                {
                    if (!Categories.Any(c => string.Equals(c, line, StringComparison.OrdinalIgnoreCase)))
                    {
                        Categories.Add(line);
                    }
                }
            }

            if (!Categories.Any())
            {
                Categories.Add("Без категории");
                SaveCategories();
            }
        }

        private void LoadBooks()
        {
            if (File.Exists(BooksFileName))
            {
                var lines = File.ReadAllLines(BooksFileName);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        var category = string.IsNullOrWhiteSpace(parts[3]) ? "Без категории" : parts[3];
                        Books.Add(new Book(parts[0], parts[1], parts[2], category));
                        if (!Categories.Any(c => string.Equals(c, category, StringComparison.OrdinalIgnoreCase)))
                        {
                            Categories.Add(category);
                        }
                    }
                    else if (parts.Length == 3)
                    {
                        Books.Add(new Book(parts[0], parts[1], parts[2], "Без категории"));
                    }
                }
            }
        }
    }
}