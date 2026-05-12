using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using BookWorm;

namespace tst
{
    public partial class LibraryForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        private const int EM_SETCUEBANNER = 0x1501;

        private LibraryManager libraryManager;
        private TextBox authorTextBox;
        private TextBox titleTextBox;
        private TextBox yearTextBox;
        private ComboBox categoryComboBox;
        private TextBox newCategoryTextBox;
        private Button addCategoryButton;
        private ComboBox filterCategoryComboBox;
        private Button filterButton;
        private Button addBookButton;
        private Button removeBookButton;
        private TextBox searchTextBox;
        private Button searchButton;
        private ListBox booksListBox;

        public LibraryForm()
        {
            this.Text = "Управление книгами";
            this.Width = 560;
            this.Height = 520;

            authorTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Width = 150
            };
            titleTextBox = new TextBox
            {
                Location = new System.Drawing.Point(170, 10),
                Width = 150
            };
            yearTextBox = new TextBox
            {
                Location = new System.Drawing.Point(330, 10),
                Width = 80
            };
            categoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(420, 10),
                Width = 110,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            newCategoryTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 40),
                Width = 220
            };
            addCategoryButton = new Button
            {
                Location = new System.Drawing.Point(240, 40),
                Text = "Добавить категорию",
                Width = 140
            };
            addCategoryButton.Click += AddCategoryButton_Click;
            filterCategoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(10, 70),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "CategoryList1"
            };
            filterButton = new Button
            {
                Location = new System.Drawing.Point(240, 70),
                Text = "Фильтр",
                Width = 100
            };
            filterButton.Click += FilterButton_Click;
            addBookButton = new Button
            {
                Location = new System.Drawing.Point(10, 100),
                Text = "Добавить",
                Width = 100
            };
            addBookButton.Click += AddBookButton_Click;
            removeBookButton = new Button
            {
                Location = new System.Drawing.Point(120, 100),
                Text = "Удалить",
                Width = 100
            };
            removeBookButton.Click += RemoveBookButton_Click;
            searchTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 130),
                Width = 200
            };
            searchButton = new Button
            {
                Location = new System.Drawing.Point(220, 130),
                Text = "Искать",
                Width = 80
            };
            searchButton.Click += SearchButton_Click;
            booksListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 160),
                Width = 520,
                Height = 300
            };

            this.Controls.Add(authorTextBox);
            this.Controls.Add(titleTextBox);
            this.Controls.Add(yearTextBox);
            this.Controls.Add(categoryComboBox);
            this.Controls.Add(newCategoryTextBox);
            this.Controls.Add(addCategoryButton);
            this.Controls.Add(filterCategoryComboBox);
            this.Controls.Add(filterButton);
            this.Controls.Add(addBookButton);
            this.Controls.Add(removeBookButton);
            this.Controls.Add(searchTextBox);
            this.Controls.Add(searchButton);
            this.Controls.Add(booksListBox);

            libraryManager = new LibraryManager();
            UpdateCategoryControls();
            UpdateBooksList();

            this.Load += (s, e) => {
                SendMessage(authorTextBox.Handle, EM_SETCUEBANNER, 0, "Автор");
                SendMessage(titleTextBox.Handle, EM_SETCUEBANNER, 0, "Название");
                SendMessage(yearTextBox.Handle, EM_SETCUEBANNER, 0, "Год");
                SendMessage(newCategoryTextBox.Handle, EM_SETCUEBANNER, 0, "Новая категория");
                SendMessage(searchTextBox.Handle, EM_SETCUEBANNER, 0, "Поиск");
            };

            this.Shown += (s, e) => {
                this.ActiveControl = null;
            };
        }

        private void UpdateCategoryControls()
        {
            categoryComboBox.Items.Clear();
            filterCategoryComboBox.Items.Clear();

            filterCategoryComboBox.Items.Add("Все категории");
            foreach (var category in libraryManager.Categories)
            {
                categoryComboBox.Items.Add(category);
                filterCategoryComboBox.Items.Add(category);
            }

            categoryComboBox.SelectedIndex = Math.Max(0, categoryComboBox.Items.IndexOf("Без категории"));
            filterCategoryComboBox.SelectedIndex = 0;
        }

        private void UpdateBooksList(IEnumerable<Book> books = null)
        {
            booksListBox.Items.Clear();
            var list = books?.ToList() ?? libraryManager.Books;
            foreach (var book in list)
            {
                booksListBox.Items.Add($"{book.Author} - {book.Title} ({book.Year}) [{book.Category}]");
            }
        }

        private void AddCategoryButton_Click(object sender, EventArgs e)
        {
            var categoryName = newCategoryTextBox.Text.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Введите имя новой категории.");
                return;
            }
            try
            {
                libraryManager.AddCategory(categoryName);
                newCategoryTextBox.Clear();
                UpdateCategoryControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (filterCategoryComboBox.SelectedItem == null)
            {
                UpdateBooksList();
                return;
            }
            string selectedCategory = filterCategoryComboBox.SelectedItem.ToString();
            if (selectedCategory == "Все категории")
            {
                UpdateBooksList();
                return;
            }
            var filteredBooks = libraryManager.FilterBooksByCategory(selectedCategory);
            UpdateBooksList(filteredBooks);
        }

        private void AddBookButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(authorTextBox.Text) || string.IsNullOrEmpty(titleTextBox.Text) ||
                string.IsNullOrEmpty(yearTextBox.Text))
            {
                MessageBox.Show("Заполните поля Автор, Название и Год.");
                return;
            }

            var category = categoryComboBox.SelectedItem?.ToString() ?? "Без категории";
            Book newBook = new Book(authorTextBox.Text.Trim(), titleTextBox.Text.Trim(), yearTextBox.Text.Trim(), category);
            try
            {
                libraryManager.AddBook(newBook);
                authorTextBox.Clear();
                titleTextBox.Clear();
                yearTextBox.Clear();
                UpdateCategoryControls();
                UpdateBooksList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveBookButton_Click(object sender, EventArgs e)
        {
            if (booksListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите книгу для удаления!");
                return;
            }

            string selectedItem = booksListBox.SelectedItem.ToString();
            int separatorIndex = selectedItem.IndexOf(" - ");

            if (separatorIndex != -1)
            {
                string author = selectedItem.Substring(0, separatorIndex).Trim();
                string rest = selectedItem.Substring(separatorIndex + 3);

                int yearStartIndex = rest.LastIndexOf(" (");
                string title = yearStartIndex != -1 ? rest.Substring(0, yearStartIndex).Trim() : rest.Trim();

                var bookToRemove = libraryManager.Books.Find(b => b.Author == author && b.Title == title);
                if (bookToRemove != null)
                {
                    try
                    {
                        libraryManager.RemoveBook(bookToRemove);
                        UpdateBooksList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchTextBox.Text))
            {
                UpdateBooksList();
                return;
            }
            var searchResults = libraryManager.SearchBooks(searchTextBox.Text.Trim());
            UpdateBooksList(searchResults);
        }
    }
}