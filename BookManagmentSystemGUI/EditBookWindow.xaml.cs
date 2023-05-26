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

using BookManagementSystemLibrary;
using System.Windows;

namespace BookManagmentSystemGUI
{
	/// <summary>
	/// Interaction logic for EditBookWindow.xaml
	/// </summary>
	public partial class EditBookWindow : Window
	{
		private Book bookToEdit;

		public EditBookWindow(ref Book book)
		{
			InitializeComponent();
			bookToEdit = book;

			TitleTextBox.Text = bookToEdit.Title;
			DescriptionTextBox.Text = bookToEdit.Description;
		}

		private void Update_Button_Click(object sender, RoutedEventArgs e)
		{
            bookToEdit.Title = TitleTextBox.Text;
			bookToEdit.Description = DescriptionTextBox.Text;

			Close();
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
