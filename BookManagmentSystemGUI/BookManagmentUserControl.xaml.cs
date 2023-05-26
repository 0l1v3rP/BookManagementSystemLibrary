using BookManagementSystemLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookManagmentSystemGUI
{
	/// <summary>
	/// Interaction logic for BookManagmentUserControl.xaml
	/// </summary>
	public partial class BookManagmentUserControl : UserControl
	{
		public BookService? bookService { get; set; }
		public BookManagmentUserControl()
		{
			InitializeComponent();
		}

		private void Clear_Button_Click(object sender, RoutedEventArgs e)
		{
			regexTextBox.Clear();
			authorTextBox.Clear();
			GenreComboBox.SelectedIndex = -1;
			FromTextBox.Clear();
			ToTextBox.Clear();
			
		}

		private void Apply_Button_Click(object sender, RoutedEventArgs e)
		{
			if (bookService != null)
			{
				BookListBox.Items.Clear();
				var filteredData = bookService.GetBooks();
				string genre = GenreComboBox.Text;
				if (!string.IsNullOrEmpty(genre))
				{
					filteredData = filteredData.Where(b => b.Genre.Equals(genre));
				}
				string author = authorTextBox.Text;

				if (!string.IsNullOrEmpty(author))
				{
					filteredData = filteredData.Where(b => b.Author.Name.Equals(author));
				}
				string regex = regexTextBox.Text;

				if (!string.IsNullOrEmpty(regex))
				{
					filteredData = filteredData.Where(b => Regex.IsMatch(b.Title, regex, RegexOptions.IgnoreCase));
				}

				if (int.TryParse(FromTextBox.Text, out int low) && int.TryParse(ToTextBox.Text, out int high))
				{
					filteredData = filteredData.Where(b => b.Reviews.Any() && b.Reviews.Average(r => r.Rating) >= low && b.Reviews.Average(r => r.Rating) <= high);
				}

				foreach (var book in filteredData)
				{
					BookListBox.Items.Add(book.Title);
				}
			}
		}


	

		private void Add_Button_Click(object sender, RoutedEventArgs e)
		{
			int prevCount = bookService == null ? 0 : bookService.BookCount;
			if (bookService != null)
			{
				var addBookWindod = new AddBookWindow(bookService);
				addBookWindod.ShowDialog();
				if (bookService.BookCount > prevCount)
				{
					BookListBox.Items.Add(bookService.GetBooks().Last().Title);
				}
			}
		}
		private void Edit_Button_Click(object sender, RoutedEventArgs e)
		{
			if (bookService != null && BookListBox.SelectedItem != null)
			{
				string selectedBookTitle = BookListBox.SelectedItem.ToString();
				Book bookToEdit = bookService.GetBooks().FirstOrDefault(b => b.Title.Equals(selectedBookTitle));

				if (bookToEdit != null)
				{
					var editBookWindow = new EditBookWindow(ref bookToEdit);
					editBookWindow.ShowDialog();
					MessageBox.Show($"{bookToEdit.Title}");
					int selectedIndex = BookListBox.SelectedIndex;
					BookListBox.Items[selectedIndex] = bookToEdit.Title;

				}
			}
		}


		private void Remove_Button_Click(object sender, RoutedEventArgs e)
		{
			if (bookService != null && BookListBox.SelectedItem != null)
			{
				string selectedBookTitle = BookListBox.SelectedItem.ToString();
				Book bookToRemove = bookService.GetBooks().FirstOrDefault(b => b.Title.Equals(selectedBookTitle));

				if (bookToRemove != null)
				{
					MessageBoxResult result = MessageBox.Show($"Are you sure you want to remove the book '{bookToRemove.Title}'?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

					if (result == MessageBoxResult.Yes)
					{
						bookService.RemoveBook(bookToRemove);
						BookListBox.Items.Remove(selectedBookTitle);
						MessageBox.Show("Book removed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					else
					{
						MessageBox.Show("Book removal canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
		}
		private void Reviews_Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
