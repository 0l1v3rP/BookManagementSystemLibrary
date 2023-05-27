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
		public BookService? BookService { get; set; }
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
			if (BookService != null)
			{
				foreach (var book in BookService)
				{
					BookListBox.Items.Add(book.Title);
				}
			}

		}

		private void Apply_Button_Click(object sender, RoutedEventArgs e)
		{
			if (BookService != null)
			{
				BookListBox.Items.Clear();
				var filteredData = BookService.GetBooks();
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
			int prevCount = BookService == null ? 0 : BookService.BookCount;
			if (BookService != null)
			{
				var addBookWindow = new AddBookWindow(BookService);
				addBookWindow.ShowDialog();
				if (BookService.BookCount > prevCount)
				{
					BookListBox.Items.Add(BookService.GetBooks().Last().Title);
				}
			}
		}

		private void Edit_Button_Click(object sender, RoutedEventArgs e)
		{
			Book? bookToEdit = SelectedBook();

			if (bookToEdit != null)
			{
				var editBookWindow = new EditBookWindow(ref bookToEdit);
				editBookWindow.ShowDialog();
				if (bookToEdit != null)
				{
					int selectedIndex = BookListBox.SelectedIndex;
					BookListBox.Items[selectedIndex] = bookToEdit.Title;
					MessageBox.Show($"Book '{bookToEdit.Title}' updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}

		private void Remove_Button_Click(object sender, RoutedEventArgs e)
		{
			if (BookService != null)
			{
				Book? bookToRemove = SelectedBook();

				if (bookToRemove != null)
				{
					MessageBoxResult result = MessageBox.Show($"Are you sure you want to remove the book '{bookToRemove.Title}'?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

					if (result == MessageBoxResult.Yes)
					{
						BookService.RemoveBook(bookToRemove);
						BookListBox.Items.Remove(bookToRemove.Title);
						MessageBox.Show("Book removed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					else
					{
						MessageBox.Show("Book removal canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
		}

		private Book? SelectedBook()
		{
			if (BookService != null && BookListBox.SelectedItem != null)
			{
				string? selectedBookTitle = BookListBox.SelectedItem.ToString() ?? "";
				return BookService.GetBooks().FirstOrDefault(b => b.Title.Equals(selectedBookTitle));
			}
			return null;
		}

		private void Reviews_Button_Click(object sender, RoutedEventArgs e)
		{
			Book? bookToShowReviews = SelectedBook();

			if (bookToShowReviews != null)
			{
				var reviewsWindow = new ReviewsWindow(bookToShowReviews.Reviews);
				reviewsWindow.ShowDialog();
			}
		}

	}
}