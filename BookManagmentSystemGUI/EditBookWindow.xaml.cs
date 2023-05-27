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
	/// Interaction logic for EditBookWindow.xaml
	/// </summary>
	public partial class EditBookWindow : Window
	{
		private readonly Book _bookToEdit;

		public EditBookWindow(ref Book book)
		{
			InitializeComponent();
			_bookToEdit = book;

			TitleTextBox.Text = _bookToEdit.Title;
			DescriptionTextBox.Text = _bookToEdit.Description;
		}

		private void Update_Button_Click(object sender, RoutedEventArgs e)
		{
            _bookToEdit.Title = TitleTextBox.Text;
			_bookToEdit.Description = DescriptionTextBox.Text;

			Close();
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
