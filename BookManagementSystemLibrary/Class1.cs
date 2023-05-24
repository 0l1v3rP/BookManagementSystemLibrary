using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManagementSystemLibrary
{
	public class Book
	{
		public List<Review> Reviews { get; set; }
		public int Id { get; set; }
		public string Description { get; set; }
		public string Genre { get; set; }
		public string Title { get; set; }

		public Author Author { get; set; }

		public Book( int id, string description,  string genre, string title, Author author)
		{
			Reviews = new List<Review>();
			Id = id;
			Description = description;
			Genre = genre;
			Title = title;
			Author = author;
		}
		public void AddReview(Review review)
		{
			Reviews.Add(review);
		}
	}

	public class Author
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public Author(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class Review
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public int Rating { get; set; }

		public Review(int id, string content, int rating)
		{
			Id = id;
			Content = content;
			Rating = rating;
		}
	}

	public class BookService
	{
		private readonly List<Book> books_;

		public BookService()
		{
			books_ = new List<Book>();
		}

		public void AddBook(Book book)
		{
			books_.Add(book);
		}

		public void DeleteBook(int id)
		{
			var book = books_.FirstOrDefault(b => b.Id == id);
			if (book != null)
			{
				books_.Remove(book);
			}
		}

		public bool UpdateBook(Book book)
		{
			var existingBook = books_.FirstOrDefault(b => b.Id == book.Id);
			if (existingBook != null)
			{
				existingBook.Title = book.Title;
				existingBook.Genre = book.Genre;
				existingBook.Author = book.Author;
				existingBook.Description = book.Description;
			}
			return existingBook != null;
		}

		public bool AddReview(Review review, Book book)
		{
			var existingBook = books_.FirstOrDefault(b => b.Id == book.Id);
			if (existingBook != null)
			{
				existingBook.AddReview(review);
			}
			return existingBook != null;
		}

		public Book SearchBook(int id)
		{
			return books_.FirstOrDefault(b => b.Id == id);
		}

		public List<Book> ListBooks()
		{
			return books_;
		}
	}



}
