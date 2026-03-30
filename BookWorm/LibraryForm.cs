using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
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
        private Button addBookButton;
        private Button removeBookButton;
        private TextBox searchTextBox;
        private Button searchButton;
        private ListBox booksListBox;
        public LibraryForm()
        {
            this.Text = "Управление книгами";
            this.Width = 500;
            this.Height = 400;
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
            addBookButton = new Button
            {
                Location = new System.Drawing.Point(10, 40),
                Text = "Добавить",
                Width = 100
            };
            addBookButton.Click += AddBookButton_Click;
            removeBookButton = new Button
            {
                Location = new System.Drawing.Point(120, 40),
                Text = "Удалить",
                Width = 100
            };
            removeBookButton.Click += RemoveBookButton_Click;
            searchTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 70),
                Width = 200
            };
            searchButton = new Button
            {
                Location = new System.Drawing.Point(220, 70),
                Text = "Искать",
                Width = 80
            };
            searchButton.Click += SearchButton_Click;
            booksListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 100),
                Width = 450,
                Height = 200
            };
            this.Controls.Add(authorTextBox);
            this.Controls.Add(titleTextBox);
            this.Controls.Add(yearTextBox);
            this.Controls.Add(addBookButton);
            this.Controls.Add(removeBookButton);
            this.Controls.Add(searchTextBox);
            this.Controls.Add(searchButton);
            this.Controls.Add(booksListBox);
            libraryManager = new LibraryManager();
            UpdateBooksList();

            this.Load += (s, e) => {
                SendMessage(authorTextBox.Handle, EM_SETCUEBANNER, 0, "Автор");
                SendMessage(titleTextBox.Handle, EM_SETCUEBANNER, 0, "Название");
                SendMessage(yearTextBox.Handle, EM_SETCUEBANNER, 0, "Год");
                SendMessage(searchTextBox.Handle, EM_SETCUEBANNER, 0, "Поиск");
            };
            this.Shown += (s, e) => {
                this.ActiveControl = null;
            };
        }
        private void UpdateBooksList()
        {
            booksListBox.Items.Clear();
            foreach (var book in libraryManager.Books)
            {
                booksListBox.Items.Add($"{book.Author} - {book.Title} ({book.Year})");
            }
        }
        private void AddBookButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(authorTextBox.Text) || string.IsNullOrEmpty(titleTextBox.Text) ||
            string.IsNullOrEmpty(yearTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            Book newBook = new Book(authorTextBox.Text, titleTextBox.Text, yearTextBox.Text);
            try
            {
                libraryManager.AddBook(newBook);
                authorTextBox.Clear();
                titleTextBox.Clear();
                yearTextBox.Clear();
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
            string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string author = parts[0].Trim();
                string title = parts[1].Trim();
                var bookToRemove = libraryManager.Books.Find(b => b.Author == author && b.Title
                == title);
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
            var searchResults = libraryManager.SearchBooks(searchTextBox.Text);
            booksListBox.Items.Clear();
            foreach (var book in searchResults)
            {
                booksListBox.Items.Add($"{book.Author} - {book.Title} ({book.Year})");
            }
        }

    }
}