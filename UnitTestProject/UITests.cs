using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UITests
    {
        private Application application;
        private AutomationBase automation;
        private Window mainWindow;

        private TextBox BookAuthorBox => FindTextBox(0);
        private TextBox BookNameBox => FindTextBox(1);
        private TextBox BookYearBox => FindTextBox(2);
        private TextBox NewCategoryBox => FindTextBox(3);
        private TextBox SearchBox => FindTextBox(4);
        private Button BtnAddCategory => FindButton("Добавить категорию");
        private Button BtnFilter => FindButton("Фильтр");
        private Button BtnAddBook => FindButton("Добавить");
        private Button BtnDeleteBook => FindButton("Удалить");
        private Button BtnSearch => FindButton("Искать");
        private ComboBox CategoryList1 => FindComboBox(0);

        private TextBox FindTextBox(int index) => mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit))[index].AsTextBox();
        private Button FindButton(string text) => mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button)).FirstOrDefault(b => b.Name == text)?.AsButton();
        private ComboBox FindComboBox(int index) => mainWindow.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.ComboBox))[index]?.AsComboBox();
        private void Sleep(int ms) => System.Threading.Thread.Sleep(ms);
        private ListBox BooksListBox => mainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.List)).AsListBox();

        [TestInitialize]
        public void TestInitialize()
        {
            var appPath = @"..\..\..\BookWorm\bin\Debug\BookWorm.exe";
            var psi = new ProcessStartInfo(appPath);
            File.Delete(@"books.txt");
            File.Delete(@"categories.txt");
            application = Application.Launch(psi);
            automation = new UIA3Automation();

            Thread.Sleep(2000);
            mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(10));
            Assert.IsNotNull(mainWindow, "Окно не найдено");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (application != null)
            {
                application.Close();
            }
            if (automation != null)
            {
                automation.Dispose();
            }
        }

        [TestMethod]
        public void TK13_AddCategory()
        {
            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();
            CategoryList1.Click();
            Sleep(500);

            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"));
        }

        [TestMethod]
        public void TK14_AddDuplicateCategory()
        {
            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();

            CategoryList1.Click();
            Sleep(500);
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"));

            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();
            Sleep(1000);

            var messageBox = application.GetAllTopLevelWindows(automation).FirstOrDefault(w =>
                w.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))?.Name.Contains("Категория с таким именем уже существует.") == true);

            Assert.IsNotNull(messageBox);
        }

        [TestMethod]
        public void TK15_AddEmptyCategory()
        {
            BtnAddCategory.Click();
            var messageBox = application.GetAllTopLevelWindows(automation).FirstOrDefault(w =>
            w.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))?.Name.Contains("Введите имя новой категории.") == true);

            Assert.IsNotNull(messageBox);
        }

        [TestMethod]
        public void TK16_AddBookWithCategory()
        {
            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();

            BookNameBox.Text = "Евгений Онегин";
            BookAuthorBox.Text = "Александр Пушкин";
            BookYearBox.Text = "1833";
            Assert.AreEqual(BookNameBox.Text, "Евгений Онегин");
            Assert.AreEqual(BookAuthorBox.Text, "Александр Пушкин");
            Assert.AreEqual(BookYearBox.Text, "1833");

            CategoryList1.Click();
            Sleep(500);
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"));
            CategoryList1.Select("Классика");

            BtnAddBook.Invoke();
            Sleep(1000);
            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Классика]")));
        }
        [TestMethod]
        public void TK17_AddBookWithoutCategory()
        {
            BookNameBox.Text = "Евгений Онегин";
            BookAuthorBox.Text = "Александр Пушкин";
            BookYearBox.Text = "1833";
            Assert.AreEqual(BookNameBox.Text, "Евгений Онегин");
            Assert.AreEqual(BookAuthorBox.Text, "Александр Пушкин");
            Assert.AreEqual(BookYearBox.Text, "1833");

            BtnAddBook.Invoke();
            Sleep(1000);

            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Без категории]")));
        }

        [TestMethod]
        public void TK18_FilterBooksByCategory()
        {
            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();

            BookNameBox.Text = "Евгений Онегин";
            BookAuthorBox.Text = "Александр Пушкин";
            BookYearBox.Text = "1833";
            Assert.AreEqual(BookNameBox.Text, "Евгений Онегин");
            Assert.AreEqual(BookAuthorBox.Text, "Александр Пушкин");
            Assert.AreEqual(BookYearBox.Text, "1833");

            CategoryList1.Click();
            Sleep(500);
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"));
            CategoryList1.Select("Классика");

            BtnAddBook.Invoke();
            Sleep(500);
            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Классика]")));

            BookNameBox.Text = "Война и мир";
            BookAuthorBox.Text = "Лев Толстой";
            BookYearBox.Text = "1869";
            Assert.AreEqual(BookNameBox.Text, "Война и мир");
            Assert.AreEqual(BookAuthorBox.Text, "Лев Толстой");
            Assert.AreEqual(BookYearBox.Text, "1869");

            BtnAddBook.Invoke();
            Sleep(500);
            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Лев Толстой - Война и мир (1869) [Без категории]")));

            var filterCategoryComboBox = FindComboBox(1);
            filterCategoryComboBox.Click();
            Sleep(500);
            Assert.IsTrue(filterCategoryComboBox.Items.Any(item => item.Name == "Классика"));
            filterCategoryComboBox.Select("Классика");

            BtnFilter.Invoke();
            Sleep(500);

            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Классика]")));
            Assert.IsFalse(BooksListBox.Items.Any(item => item.Name.Contains("Лев Толстой - Война и мир (1869) [Без категории]")));
        }

        [TestMethod]
        public void TK19_FilterBooksByCategorySearch()
        {
            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();

            BookNameBox.Text = "Евгений Онегин";
            BookAuthorBox.Text = "Александр Пушкин";
            BookYearBox.Text = "1833";
            Assert.AreEqual(BookNameBox.Text, "Евгений Онегин");
            Assert.AreEqual(BookAuthorBox.Text, "Александр Пушкин");
            Assert.AreEqual(BookYearBox.Text, "1833");

            CategoryList1.Click();
            Sleep(500);
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"));
            CategoryList1.Select("Классика");

            BtnAddBook.Invoke();
            Sleep(500);
            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Классика]")));

            BookNameBox.Text = "Война и мир";
            BookAuthorBox.Text = "Лев Толстой";
            BookYearBox.Text = "1869";
            Assert.AreEqual(BookNameBox.Text, "Война и мир");
            Assert.AreEqual(BookAuthorBox.Text, "Лев Толстой");
            Assert.AreEqual(BookYearBox.Text, "1869");

            BtnAddBook.Invoke();
            Sleep(500);
            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Лев Толстой - Война и мир (1869) [Без категории]")));

            SearchBox.Text = "Классика";
            Assert.AreEqual(SearchBox.Text, "Классика");

            BtnSearch.Invoke();
            Sleep(500);

            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Классика]")));
            Assert.IsFalse(BooksListBox.Items.Any(item => item.Name.Contains("Лев Толстой - Война и мир (1869) [Без категории]")));
        }

        [TestMethod]
        public void TK20_SaveBooksWithCategory()
        {
            NewCategoryBox.Text = "Классика";
            BtnAddCategory.Click();

            BookNameBox.Text = "Евгений Онегин";
            BookAuthorBox.Text = "Александр Пушкин";
            BookYearBox.Text = "1833";
            Assert.AreEqual(BookNameBox.Text, "Евгений Онегин");
            Assert.AreEqual(BookAuthorBox.Text, "Александр Пушкин");
            Assert.AreEqual(BookYearBox.Text, "1833");

            CategoryList1.Click();
            Sleep(500);
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"));
            CategoryList1.Select("Классика");

            BtnAddBook.Invoke();
            Sleep(500);
            Assert.IsTrue(BooksListBox.Items.Any(item => item.Name.Contains("Александр Пушкин - Евгений Онегин (1833) [Классика]")));

            string booksFilePath = @"books.txt";
            Assert.IsTrue(File.Exists(booksFilePath));
            
            string fileContent = File.ReadAllText(booksFilePath);
            Assert.IsTrue(fileContent.Contains("Александр Пушкин|Евгений Онегин|1833|Классика"));
        }

        [TestMethod]
        public void TK21_SaveCategoryToFile()
        {
            NewCategoryBox.Text = "Классика";
            Assert.AreEqual(NewCategoryBox.Text, "Классика");

            BtnAddCategory.Click();
            Sleep(500);

            string categoriesFilePath = @"categories.txt";
            Assert.IsTrue(File.Exists(categoriesFilePath));
            
            string fileContent = File.ReadAllText(categoriesFilePath);
            Assert.IsTrue(fileContent.Contains("Без категории"));
            Assert.IsTrue(fileContent.Contains("Классика"));
        }

        [TestMethod]
        public void TK22_LoadCategoriesFromFile()
        {
            if (application != null)
            {
                application.Close();
            }
            if (automation != null)
            {
                automation.Dispose();
            }

            string categoriesFilePath = @"categories.txt";
            File.WriteAllText(categoriesFilePath, "Без категории\nКлассика\nФантастика");

            var appPath = @"..\..\..\BookWorm\bin\Debug\BookWorm.exe";
            var psi = new ProcessStartInfo(appPath);
            application = Application.Launch(psi);
            automation = new UIA3Automation();
            Thread.Sleep(2000);
            mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(10));

            CategoryList1.Click();
            Sleep(500);
            
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Без категории"), "Категория 'Без категории' не найдена");
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Классика"), "Категория 'Классика' не найдена");
            Assert.IsTrue(CategoryList1.Items.Any(item => item.Name == "Фантастика"), "Категория 'Фантастика' не найдена");
        }
    }
}
