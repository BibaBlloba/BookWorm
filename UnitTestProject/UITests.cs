using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [TestInitialize]
        public void TestInitialize()
        {
            // Запуск приложения
            var appPath = @"..\..\..\BookWorm\bin\Debug\BookWorm.exe";
            var psi = new ProcessStartInfo(appPath);
            application = Application.Launch(psi);
            automation = new UIA3Automation();

            // Ожидание загрузки окна
            Thread.Sleep(2000);
            mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(10));
            Assert.IsNotNull(mainWindow, "Главное окно не найдено");
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
            NewCategoryBox.Text = "Новая категория";
            BtnAddCategory.Click();

            bool exists = CategoryList1.Items.Any(item => item.Name == "Без категории");
            Assert.IsTrue(exists);

            System.Threading.Thread.Sleep(10000);
        }

        private TextBox FindTextBox(int index) => mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit))[index].AsTextBox();
        private Button FindButton(string text) => mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Button)).FirstOrDefault(b => b.Name.Contains(text))?.AsButton();
        private ComboBox FindComboBox(int index) => mainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.ComboBox)).ElementAtOrDefault(index)?.AsComboBox();
        //private ComboBox FindComboBox(string text) => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(text)).AsComboBox();

    }
}
