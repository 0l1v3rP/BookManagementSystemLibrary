using BookManagementSystemLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BookManagmentSystemGUI
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        private BookService _bookService;
		private Author? _author;
		private string currentId;
		private bool changingText = false;

		public AddBookWindow(BookService bookService)
        {
            InitializeComponent();
            _bookService = bookService;
			int newId = 0;
			while (_bookService.Any(b => b.Id == newId))
			{
				++newId;
			}
			IdTextBox.Text = newId.ToString();
			currentId = newId.ToString();
			_author = null;

		}

		private void AddAuthor_Button_Click(object sender, RoutedEventArgs e)
		{
			var addAuthorWindow = new AddAuthorWindow(_bookService.GetAuthors());
			addAuthorWindow.ShowDialog();

			var newAuthor = addAuthorWindow.NewAuthor;
			if (newAuthor != null)
			{
				_author = newAuthor;
			}
		}

		private void OK_Button_Click(object sender, RoutedEventArgs e)
		{
			if (!int.TryParse(IdTextBox.Text, out int bookId))
			{
				MessageBox.Show("Please enter a valid ID.");
				return;
			}

			string title = TitleTextBox.Text;
			if (string.IsNullOrWhiteSpace(title))
			{
				MessageBox.Show("Please enter a valid title.");
				return;
			}

			if (!(GenreComboBox.SelectedItem is ComboBoxItem genreItem) || string.IsNullOrWhiteSpace(genreItem.Content.ToString()))
			{
				MessageBox.Show("Please select a valid genre.");
				return;
			}
			string genre = genreItem.Content.ToString();

			string description = DescriptionTextBox.Text;

			if (_author == null)
			{
				MessageBox.Show("Please add or select an author.");
				return;
			}

			try
			{
				_bookService.AddBook(bookId, description, genre, title, _author, _author?.Id);
				MessageBox.Show("Book added successfully!");
				this.Close();
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show($"Failed to add the book. Error: {ex.Message}");
			}
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


		private void IdTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (changingText)
				return;

			if (string.IsNullOrEmpty(IdTextBox.Text))
			{
				currentId = IdTextBox.Text;
			}
			else
			{
				if (!int.TryParse(IdTextBox.Text, out int id) || _bookService.Any(b => b.Id == id))
				{
					changingText = true;
					IdTextBox.Text = currentId;
					changingText = false;
				}
				else
				{
					currentId = IdTextBox.Text;
				}
			}
		}


	}
}
