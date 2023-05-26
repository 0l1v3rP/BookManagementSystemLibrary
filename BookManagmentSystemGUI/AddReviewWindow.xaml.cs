using System.Windows;
using System.Windows.Controls;

namespace BookManagmentSystemGUI
{
	/// <summary>
	/// Interaction logic for AddReviewWindow.xaml
	/// </summary>
	public partial class AddReviewWindow : Window
	{
		public string ReviewContent { get; private set; } = "";
		public int Rating { get; private set; } = 0;

		public AddReviewWindow()
		{
			InitializeComponent();
		}

		private void Add_Button_Click(object sender, RoutedEventArgs e)
		{
			ReviewContent = ReviewContentTextBox.Text;

			if (RatingComboBox.SelectedItem is ComboBoxItem selectedRatingItem && int.TryParse(selectedRatingItem.Content.ToString(), out int selectedRating))
			{
				Rating = selectedRating;
			}
			else
			{
				MessageBox.Show("Please select a valid rating.", "Invalid Rating", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			DialogResult = true;
			Close();
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
