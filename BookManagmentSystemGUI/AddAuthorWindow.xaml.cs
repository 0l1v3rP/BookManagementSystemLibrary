using BookManagementSystemLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookManagmentSystemGUI
{
	/// <summary>
	/// Interaction logic for AddAuthorWindow.xaml
	/// </summary>
	public partial class AddAuthorWindow : Window
	{
		private IEnumerable<Author> _authors;
		public Author? NewAuthor { get; private set; }



		public AddAuthorWindow(IEnumerable<Author> authors)
		{
			InitializeComponent();
			_authors = authors;
			AuthorsListBox.ItemsSource = _authors;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			if (NameTextBox.Text != "")
			{
				int newId = _authors.Any() ? _authors.Max(a => a.Id) + 1 : 1;

				NewAuthor = new Author(newId, NameTextBox.Text);
				_authors = _authors.Append(NewAuthor);

				AuthorsListBox.ItemsSource = null;
				AuthorsListBox.ItemsSource = _authors;
			}
		}


		private void OK_Click(object sender, RoutedEventArgs e)
		{
			var selectedAuthor = AuthorsListBox.SelectedItem as Author;

			NewAuthor = selectedAuthor ?? NewAuthor;
            if (NewAuthor != null)
            {
				
				this.Close();
			}
			else
			{
				MessageBox.Show("Choose author!");
			}
		}


		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			NewAuthor = null;
			this.Close();
		}
	}

}
