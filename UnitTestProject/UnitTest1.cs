using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookWorm.Tests
{
    [TestClass]
    public class LibraryManagerTests
    {
        private const string TestFileName = "books.txt";
        private LibraryManager _libraryManager;

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
            _libraryManager = new LibraryManager();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }

        [TestMethod]
        public void AddBook_test()
        {
            // Arrange
            var book = new Book("Александр Пушкин", "Евгений Онегин", "1833");

            // Act
            _libraryManager.AddBook(book);

            // Assert
            Assert.AreEqual(1, _libraryManager.Books.Count);
            Assert.AreEqual(book, _libraryManager.Books[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddBook_Null_ThrowsArgumentNullException()
        {
            // Act
            _libraryManager.AddBook(null);
        }

        [TestMethod]
        public void RemoveBook_test()
        {
            // Arrange
            var book = new Book("Александр Пушкин", "Евгений Онегин", "1833");

            // Act
            _libraryManager.AddBook(book);
            _libraryManager.RemoveBook(book);

            // Assert
            Assert.AreEqual(0, _libraryManager.Books.Count); 
            Assert.IsFalse(_libraryManager.Books.Contains(book));

            var savedContent = File.ReadAllLines(TestFileName);
            Assert.AreEqual(0, savedContent.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveBook_Null_ThrowsArgumentNullException()
        {
            // Act
            _libraryManager.RemoveBook(null);
        }

        [TestMethod]
        public void RemoveBook_NonExistingBook_DoesNotThrowAndDoesNotChangeList()
        {
            // Arrange
            var book1 = new Book("Лев Толстой", "Война и мир", "1869");
            var book2 = new Book("Фёдор Достоевский", "Преступление и наказание", "1866");

            // Act
            _libraryManager.AddBook(book1);
            _libraryManager.RemoveBook(book2);

            // Assert
            Assert.AreEqual(1, _libraryManager.Books.Count);
        }

        [TestMethod]
        public void SearchBooks_Author_test()
        {
            // Arrange
            _libraryManager.AddBook(new Book("Лев Толстой", "Война и мир", "1869"));
            _libraryManager.AddBook(new Book("Фёдор Достоевский", "Преступление и наказание", "1866"));
            _libraryManager.AddBook(new Book("Лев Толстой", "Анна Каренина", "1877"));

            // Act
            var results = _libraryManager.SearchBooks("Лев");

            // Assert
            Assert.AreEqual(2, results.Count);
            foreach (var book in results)
            {
                Assert.AreEqual("Лев Толстой", book.Author);
            }
        }

        [TestMethod]
        public void SearchBooks_Title_test()
        {
            // Arrange
            _libraryManager.AddBook(new Book("Лев Толстой", "Война и мир", "1869"));
            _libraryManager.AddBook(new Book("Фёдор Достоевский", "Преступление и наказание", "1866"));
            _libraryManager.AddBook(new Book("Лев Толстой", "Анна Каренина", "1877"));

            // Act
            var results = _libraryManager.SearchBooks("Прест");

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Преступление и наказание", results[0].Title);

        }

        [TestMethod]
        public void SearchBooks_NoMatches_test()
        {
            // Arrange
            _libraryManager.AddBook(new Book("Лев Толстой", "Война и мир", "1869"));
            _libraryManager.AddBook(new Book("Фёдор Достоевский", "Преступление и наказание", "1866"));

            // Act
            var results = _libraryManager.SearchBooks("qwe");

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void SearchBooks_EmptyQuery_ReturnsAllBooks()
        {
            // Arrange
            _libraryManager.AddBook(new Book("Лев Толстой", "Война и мир", "1869"));
            _libraryManager.AddBook(new Book("Фёдор Достоевский", "Преступление и наказание", "1866"));

            // Act
            var results = _libraryManager.SearchBooks("");

            // Assert
            Assert.AreEqual(2, results.Count);
        }
    }
}
