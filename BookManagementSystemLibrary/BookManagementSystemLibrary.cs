using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BookManagementSystemLibrary
{
	public class Book
	{
		public IList<Review> Reviews { get; set; }
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

	public struct Review
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


	public class BookService : IEnumerable<Book>
	{
		public delegate IEnumerable<Book> SearchDelegate(string query);

		public SearchDelegate SearchByAuthor;
		public SearchDelegate SearchByGenre;
		public SearchDelegate SearchByRating;

		private readonly List<Book> _books;


		public int BookCount => _books.Count;

		public int AuthorCount => _books.Select(b => b.Author).Distinct().Count();

		public string[] this[Author author]
		{
			get
			{
				return _books.Where(b => b.Author == author).Select(b => b.Title).ToArray();
			}
		}

		public string[] this[string genre]
		{
			get
			{
				return _books.Where(b => b.Genre == genre).Select( b => b.Title).ToArray();
			}
		}
		public void Clear()
		{
			_books.Clear();
		}
		public BookService()
		{
			_books = new List<Book>();

			SearchByAuthor = query =>
			{
				int authorId;
				bool success = Int32.TryParse(query, out authorId);
				if (success)
				{
					return _books.Where(b => b.Author.Id == authorId);
				}
				return new List<Book>();
			};

			SearchByGenre = query => _books.Where(b => b.Genre == query);

			SearchByRating = query =>
			{
				int rating;
				bool success = Int32.TryParse(query, out rating);
				if (success)
				{
					return _books.Where(b => b.Reviews.Average(r => r.Rating) >= rating);
				}
				return new List<Book>();
			};
		}
		public void AddBook(int bookId, string description, string genre, string title, Author? author, int? authorID)
		{
			if ((author == null && authorID == null) ||
				(author == null && GetAuthors().FirstOrDefault(a => a.Id == authorID) == null) ||
				(author != null && GetAuthors().FirstOrDefault(a => a.Id == author.Id) != null))
			{
				throw new ArgumentException("Invalid author data.");
			}

			if (Contains(bookId))
			{
				throw new ArgumentException("Invalid book Id.");
			}

			Author bookAuthor = author ?? GetAuthors().FirstOrDefault(a => a.Id == authorID.Value);
			_books.Add(new Book(bookId, description, genre, title, bookAuthor));
		}


		public bool AddBook(Book book)
		{ 
            if (_books.Where(b => b.Id != book.Id) != null)
            {
				_books.Add(book);
				return true;
			}
			return false;
            
		}

		public void DeleteBook(int id)
		{
			var book = _books.FirstOrDefault(b => b.Id == id);
			if(book != null)
			_books.Remove(book);
			
		}

		public void UpdateBook(Book book)
		{
			if (string.IsNullOrEmpty(book.Title))
			{
				throw new ArgumentException("Book title cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(book.Genre))
			{
				throw new ArgumentException("Book genre cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(book.Description))
			{
				throw new ArgumentException("Book description cannot be null or empty.");
			}
			if (book.Author == null)
			{
				throw new ArgumentException("Book author cannot be null.");
			}
			var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
			if (existingBook == null)
			{
				throw new ArgumentException("There's no book with given id");
			}
				existingBook.Title = book.Title;
				existingBook.Genre = book.Genre;
				existingBook.Author = book.Author;
				existingBook.Description = book.Description;
			
		}

		public void UpdateBook(Book oldbook, Book newBook)
		{
			if (string.IsNullOrEmpty(newBook.Title))
			{
				throw new ArgumentException("Book title cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(newBook.Genre))
			{
				throw new ArgumentException("Book genre cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(newBook.Description))
			{
				throw new ArgumentException("Book description cannot be null or empty.");
			}
			if (newBook.Author == null)
			{
				throw new ArgumentException("Book author cannot be null.");
			}
			_books[oldbook.Id] = newBook;
		}


		public bool AddReview(Review review, int id)
		{

			var existingBook = _books.FirstOrDefault(b => b.Id == id);
			if (existingBook != null && existingBook.Reviews.Where(r => r.Id != review.Id) != null)
			{
				existingBook.AddReview(review);
			}
			return existingBook != null;
		}
		
		public Book SearchBook(int bookId)
		{
			if (Contains(bookId))
			{
				throw new ArgumentException("Invalid id");
			}
			return _books.FirstOrDefault(b => b.Id == bookId);
		}

		public IEnumerator<Book> GetEnumerator()
		{
			return _books.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerable<Book> GetBooksByAuthorID(int authorID)
		{
			return _books.Where(b => b.Author.Id == authorID);
		}

		public IEnumerable<Book> GetBooksByGenre(string genre)
		{
			return _books.Where(b => b.Genre == genre);
		}

		public IList<Review> GetReviews(int id)
		{
			return _books.FirstOrDefault(b => b.Id  == id).Reviews;
		}
		public IEnumerable<Book> GetBooksByRating(int rating)
		{
			return _books.Where(b => b.Reviews.Average(b => b.Rating) <= rating);
		}

		public IEnumerable<Author> GetAuthors()
		{
			return _books.Select(b => b.Author);
		}

		public bool Contains(int id)
		{
			return _books.Contains(_books.FirstOrDefault(b => b.Id == id));
		}
		public bool Contains(Book book)
		{
			return _books.Contains(book);
		}


	}
}
