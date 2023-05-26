﻿using BookManagementSystemLibrary;
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
	/// Interaction logic for ReviewsWindow.xaml
	/// </summary>
	public partial class ReviewsWindow : Window
	{
		private IList<Review> _reviews;

		public ReviewsWindow(IList<Review> reviews)
		{
			_reviews = reviews;
			InitializeComponent();

			CountTextBlock.Text = $"Count: {_reviews.Count}";
			AverageRatingTextBlock.Text = $"Average Rating: {_reviews.Average(r => r.Rating).ToString("0.00")}";
			ReviewsListBox.ItemsSource = _reviews;
		}
		//TODO
		private void Add_Button_Click(object sender, RoutedEventArgs e)
		{
			var addReviewWindow = new AddReviewWindow();
			if (addReviewWindow.ShowDialog() == true)
			{
				int id = GenerateNewReviewId();
				string content = addReviewWindow.ReviewContent;
				int rating = addReviewWindow.Rating;

				
				var newReview = new Review(id, content, rating);

				_reviews.Add(newReview);

				CountTextBlock.Text = $"Count: {_reviews.Count}";
				AverageRatingTextBlock.Text = $"Average Rating: {_reviews.Average(r => r.Rating)}";
			}
		}
		//TODO
		private void Remove_Button_Click(object sender, RoutedEventArgs e)
		{
			if (ReviewsListBox.SelectedItem is Review selectedReview)
			{
				_reviews.Remove(selectedReview);
				ReviewsListBox.Items.Remove(ReviewsListBox.SelectedItem);
				CountTextBlock.Text = $"Count: {_reviews.Count}";
				AverageRatingTextBlock.Text = $"Average Rating: {_reviews.Average(r => r.Rating)}";
			}
		}

		private int GenerateNewReviewId()
		{
			int newId = 1;
			if (_reviews.Count > 0)
			{
				newId = _reviews.Max(r => r.Id) + 1;
                while (_reviews.Any(r => r.Id == newId))
				{
					++newId;
                }
            }
			
			return newId;
		}
	}

}
