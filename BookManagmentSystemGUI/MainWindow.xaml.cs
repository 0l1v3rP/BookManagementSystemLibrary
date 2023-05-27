using BookManagementSystemLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookManagmentSystemGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly BookService _bookService;
		public MainWindow()
		{
			InitializeComponent();
			_bookService = new BookService();
			bookManagment.BookService = _bookService;
		}

		private void New_Click(object sender, RoutedEventArgs e)
		{
			if (_bookService.BookCount > 0)
			{
				MessageBoxResult result = MessageBox.Show("Reset library ?", "Confirm", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					_bookService.Clear();
				}
			}

		}
		private void Open_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new();
			if (openFileDialog.ShowDialog() == true)
			{
				FileInfo csvFile = new(openFileDialog.FileName);
				if (csvFile.Extension.ToLower() != ".csv")
				{
					MessageBox.Show("Invalid file type. Please select a CSV file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
				{
					_bookService.Load(csvFile);
					bookManagment.BookService = _bookService;
					foreach(Book book in _bookService)
					{
						bookManagment.BookListBox.Items.Add(book.Title);
					}

				}
			}
		}

		private void Save_As_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new();
			if (saveFileDialog.ShowDialog() == true)
			{
				try
				{
					FileInfo csvFile = new(saveFileDialog.FileName);

					_bookService.Save(csvFile);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Failed to save the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
		private void About_Click(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow() ;
			aboutWindow.Show();
		}

	}
}
