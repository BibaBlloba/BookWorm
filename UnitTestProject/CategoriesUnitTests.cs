using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookWorm;

namespace BookWorm.Tests
{
    [TestClass]
    public class CategoriesUnitTests
    {
        private const string BooksTestFileName = "books.txt";
        private const string CategoriesTestFileName = "categories.txt";
        private LibraryManager _libraryManager;

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(BooksTestFileName))
            {
                File.Delete(BooksTestFileName);
            }
            if (File.Exists(CategoriesTestFileName))
            {
                File.Delete(CategoriesTestFileName);
            }
            _libraryManager = new LibraryManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(BooksTestFileName))
            {
                File.Delete(BooksTestFileName);
            }
            if (File.Exists(CategoriesTestFileName))
            {
                File.Delete(CategoriesTestFileName);
            }
        }

        [TestMethod]
        public void AddCategory()
        {
            // Arrange
            string categoryName = "Фантастика";

            // Act
            _libraryManager.AddCategory(categoryName);

            // Assert
            Assert.IsTrue(_libraryManager.Categories.Contains(categoryName));
            Assert.IsTrue(File.Exists(CategoriesTestFileName));
            var savedCategories = File.ReadAllLines(CategoriesTestFileName);
            Assert.IsTrue(savedCategories.Contains(categoryName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddCategory_EmptyName()
        {
            // Act
            _libraryManager.AddCategory("");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddCategory_Duplicate()
        {
            // Arrange
            string categoryName = "Фантастика";
            _libraryManager.AddCategory(categoryName);

            // Act
            _libraryManager.AddCategory(categoryName);
        }

        [TestMethod]
        public void AddBook_WithCategory()
        {
            // Arrange
            string category = "Научная фантастика";
            var book = new Book("Артур Кларк", "2001: Космическая одиссея", "1968", category);

            // Act
            _libraryManager.AddBook(book);

            // Assert
            Assert.AreEqual(1, _libraryManager.Books.Count);
            Assert.AreEqual(category, _libraryManager.Books[0].Category);
            Assert.IsTrue(_libraryManager.Categories.Contains(category));
        }

        [TestMethod]
        public void AddBook_DefaultCategory()
        {
            // Arrange
            var book = new Book("Лев Толстой", "Война и мир", "1869");

            // Act
            _libraryManager.AddBook(book);

            // Assert
            Assert.AreEqual("Без категории", _libraryManager.Books[0].Category);
        }

        [TestMethod]
        public void FilterBooks()
        {
            // Arrange
            var book1 = new Book("Лев Толстой", "Война и мир", "1869", "Классика");
            var book2 = new Book("Артур Кларк", "2001: Космическая одиссея", "1968", "Фантастика");
            var book3 = new Book("Фёдор Достоевский", "Преступление и наказание", "1866", "Классика");
            _libraryManager.AddBook(book1);
            _libraryManager.AddBook(book2);
            _libraryManager.AddBook(book3);

            // Act
            var filteredBooks = _libraryManager.FilterBooksByCategory("Классика");

            // Assert
            Assert.AreEqual(2, filteredBooks.Count);
        }

        [TestMethod]
        public void FilterBooks_AllCategories()
        {
            // Arrange
            var book1 = new Book("Лев Толстой", "Война и мир", "1869", "Классика");
            var book2 = new Book("Артур Кларк", "2001: Космическая одиссея", "1968", "Фантастика");
            _libraryManager.AddBook(book1);
            _libraryManager.AddBook(book2);

            // Act
            var filteredBooks = _libraryManager.FilterBooksByCategory("Все категории");

            // Assert
            Assert.AreEqual(2, filteredBooks.Count);
        }

        [TestMethod]
        public void FilterBooks_EmptyCategory()
        {
            // Arrange
            var book1 = new Book("Лев Толстой", "Война и мир", "1869", "Классика");
            var book2 = new Book("Артур Кларк", "2001: Космическая одиссея", "1968", "Фантастика");
            _libraryManager.AddBook(book1);
            _libraryManager.AddBook(book2);

            // Act
            var filteredBooks = _libraryManager.FilterBooksByCategory("");

            // Assert
            Assert.AreEqual(2, filteredBooks.Count);
        }

        [TestMethod]
        public void SearchBooks_InCategory()
        {
            // Arrange
            var book1 = new Book("Лев Толстой", "Война и мир", "1869", "Классика");
            var book2 = new Book("Артур Кларк", "2001: Космическая одиссея", "1968", "Фантастика");
            _libraryManager.AddBook(book1);
            _libraryManager.AddBook(book2);

            // Act
            var searchResults = _libraryManager.SearchBooks("Классика");

            // Assert
            Assert.AreEqual(1, searchResults.Count);
            Assert.AreEqual("Классика", searchResults[0].Category);
        }
    }
}
