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

		}

		private void Edit_Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Remove_Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Details_Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ShowReviews_Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
