using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace BookManagementSystemLibrary
{
	/// <summary>
	/// Represents a book with properties such as Reviews, Id, Description, Genre, Title and Author.
	/// </summary>
	public class Book
	{
		public IList<Review> Reviews { get; set; }
		public int Id { get; set; }
		public string Description { get; set; }
		public string Genre { get; set; }
		public string Title { get; set; }

		public Author Author { get; set; }

		/// <summary>
		/// Creates a new instance of Book class with specified parameters.
		/// </summary>
		/// <param name="id">The book's id.</param>
		/// <param name="description">The book's description.</param>
		/// <param name="genre">The book's genre.</param>
		/// <param name="title">The book's title.</param>
		/// <param name="author">The author of the book.</param>
		public Book(int id, string description, string genre, string title, Author author)
		{
			Reviews = new List<Review>();
			Id = id;
			Description = description;
			Genre = genre;
			Title = title;
			Author = author;
		}

		/// <summary>
		/// Adds a review to the book's list of reviews.
		/// </summary>
		/// <param name="review">The review to be added.</param>
		public void AddReview(Review review)
		{
			Reviews.Add(review);
		}
	}

	/// <summary>
	/// Represents an author with properties such as Id and Name.
	/// </summary>
	public class Author
	{
		public int Id { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// Creates a new instance of Author class with specified parameters.
		/// </summary>
		/// <param name="id">The author's id.</param>
		/// <param name="name">The author's name.</param>
		public Author(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	/// <summary>
	/// Represents a review with properties such as Id, Content and Rating.
	/// </summary>
	public struct Review
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public int Rating { get; set; }

		/// <summary>
		/// Creates a new instance of Review struct with specified parameters.
		/// </summary>
		/// <param name="id">The review's id.</param>
		/// <param name="content">The content of the review.</param>
		/// <param name="rating">The rating given in the review.</param>
		public Review(int id, string content, int rating)
		{
			Id = id;
			Content = content;
			Rating = rating;
		}
	}



	public class BookService : IEnumerable<Book>
	{
		/// <summary>
		/// Delegate declaration for a search operation. This delegate takes a string query and returns an IEnumerable of Book objects that match the query.
		/// </summary>
		/// <param name="query">The search query.</param>
		/// <returns>An IEnumerable of Book objects that match the query.</returns>
		public delegate IEnumerable<Book> SearchDelegate(string query);

		/// <summary>
		/// Property using the SearchDelegate. This delegate is used to define a method that searches for books by author.
		/// </summary>
		public SearchDelegate SearchByAuthor;

		/// <summary>
		/// Property using the SearchDelegate. This delegate is used to define a method that searches for books by genre.
		/// </summary>
		public SearchDelegate SearchByGenre;

		/// <summary>
		/// Property using the SearchDelegate. This delegate is used to define a method that searches for books by rating.
		/// </summary>
		public SearchDelegate SearchByRating;


		private readonly List<Book> _books;

		/// <summary>
		/// count of all books
		/// </summary>
		public int BookCount => _books.Count;

		/// <summary>
		/// count of all different authors
		/// </summary>

		public int AuthorCount => _books.Select(b => b.Author).Distinct().Count();



		/// <summary>
		/// returns a title of the book by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string[] this[int id]
		{
			get
			{
				return _books.Where(b => b.Id == id).Select(b => b.Title).ToArray();
			}
		}

		/// <summary>
		/// returns titles of all books that have same author
		/// </summary>
		/// <param name="author"></param>
		/// <returns></returns>

		public string[] this[Author author]
		{
			get
			{
				return _books.Where(b => b.Author == author).Select(b => b.Title).ToArray();
			}
		}
		/// <summary>
		/// returns titles of all books that have same genre
		/// </summary>
		/// <param name="genre"></param>
		/// <returns></returns>

		public string[] this[string genre]
		{
			get
			{
				return _books.Where(b => b.Genre == genre).Select(b => b.Title).ToArray();
			}
		}
		/// <summary>
		/// clear list of books
		/// </summary>
		public void Clear()
		{
			_books.Clear();
		}
		/// <summary>
		/// initialize BookService class and given delegate functions
		/// </summary>
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
		/// <summary>
		/// add a book with given paramters, if the author already exists you can reference him by author id else you need to add new author
		/// </summary>
		/// <param name="bookId"></param>
		/// <param name="description"></param>
		/// <param name="genre"></param>
		/// <param name="title"></param>
		/// <param name="author"></param>
		/// <param name="authorID"></param>
		/// <exception cref="ArgumentException"></exception>
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

			Author? bookAuthor;
			if (author != null)
			{
				bookAuthor = author;
			}
			else if (authorID != null)
			{
				bookAuthor = GetAuthors().FirstOrDefault(a => a.Id == authorID.Value);
			}
			else
			{
				throw new ArgumentException("Both author and authorId cannot be null.");
			}

			if (bookAuthor == null)
			{
				throw new ArgumentException("Invalid author data.");
			}

			_books.Add(new Book(bookId, description, genre, title, bookAuthor));
		}


		/// <summary>
		/// tries to add a book to a list of books, if there's already a book with the same id
		/// then the function will retunr false otherwise true, 
		/// also exception will be thrown if one og the following is null or empty: title, genre, description, author
		/// </summary>
		/// <param name="book"></param>
		/// <returns></returns>
		public void AddBook(Book book)
		{
			checkIfValid(book);
			if (_books.Any(b => b.Id == book.Id))
			{
				throw new ArgumentException("book with given Id already exists");
			}
			_books.Add(book);
		}

		/// <summary>
		/// deletes a book by book id
		/// </summary>
		/// <param name="id"></param>
		public void DeleteBook(int id)
		{
			var book = _books.FirstOrDefault(b => b.Id == id);
			if (book != null)
				_books.Remove(book);

		}
		/// <summary>
		/// Updates book that has same id as book passed as parameter
		/// also exception will be thrown if one og the following is null or empty: title, genre, description, author
		/// exception will also be thrown if there's no book with such id
		/// </summary>
		/// <param name="book"></param>
		/// <exception cref="ArgumentException"></exception>
		public void UpdateBook(Book book)
		{
			checkIfValid(book);
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

		private void checkIfValid(Book book)
		{
			if (string.IsNullOrEmpty(book.Title))
			{
				throw new ArgumentException("Book title cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(book.Genre))
			{
				throw new ArgumentException("Book genre cannot be null or empty.");
			}
			if (book.Author == null)
			{
				throw new ArgumentException("Book author cannot be null.");
			}
			if (!AllowedGenres(book.Genre))
			{
				throw new ArgumentException($"Wrong genre {book.Genre}");
			}
		}

		/// <summary>
		/// updates a old book with a new book by their id's,
		/// also exception will be thrown if one og the following is null or empty: title, genre, description, author
		/// exception will also be thrown if id's won't match
		/// </summary>
		/// <param name="oldbook"></param>
		/// <param name="newBook"></param>
		/// <exception cref="ArgumentException"></exception>
		public void UpdateBook(Book oldBook, Book newBook)
		{
			checkIfValid(newBook);

			if (oldBook.Id != newBook.Id)
			{
				throw new ArgumentException("Ids of old and new book must match.");
			}

			int index = _books.FindIndex(b => b.Id == oldBook.Id);
			if (index != -1)
			{
				_books[index] = newBook;
			}
		}

		/// <summary>
		/// adds a review for a given book that is referenced by id, if the book doesn't exists
		/// or the review with the given id already exists, then the function will return false else true
		/// </summary>
		/// <param name="review"></param>
		/// <param name="id">book id</param>
		/// <returns></returns>
		public bool AddReview(Review review, int id)
		{
			Book? existingBook = _books.FirstOrDefault(b => b.Id == id);
			if (existingBook != null && existingBook.Reviews.All(r => r.Id != review.Id))
			{
				existingBook.AddReview(review);
				return true;
			}
			return false;
		}



		/// <summary>
		/// searches book by id, if the id is not in book list then the ArgumentException("Invalid id") will be thrown,
		/// if the book exists, then the book will be returned
		/// </summary>
		/// <param name="bookId"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public Book SearchBook(int bookId)
		{
			if (!Contains(bookId))
			{
				throw new ArgumentException("Invalid id");
			}
			return _books.First(b => b.Id == bookId);
		}
		/// <summary>
		/// returns an  Enumerator for books
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Book> GetEnumerator()
		{
			return _books.GetEnumerator();
		}

		public IEnumerable<Book> GetBooks()
		{
			return _books;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		/// <summary>
		/// returns Collections of books by author Id
		/// </summary>
		/// <param name="authorID"></param>
		/// <returns></returns>
		public IEnumerable<Book> GetBooksByAuthorID(int authorID)
		{
			return _books.Where(b => b.Author.Id == authorID);
		}
		/// <summary>
		/// returns Collections of books by genre
		/// </summary>
		/// <param name="genre"></param>
		/// <returns></returns>
		public IEnumerable<Book> GetBooksByGenre(string genre)
		{
			return _books.Where(b => b.Genre == genre);
		}

		/// <summary>
		/// returns a list of reviews of given book by id of that book, if no such book exists  returns null
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IList<Review>? GetReviewsOrNull(int id)
		{
			Book? book = _books.FirstOrDefault(b => b.Id == id);
			if (book != null)
			{
				return book.Reviews;
			}
			return null;
		}

		/// <summary>
		/// returns a collection of books by their rating
		/// </summary>
		/// <param name="rating"></param>
		/// <returns></returns>
		public IEnumerable<Book> GetBooksByRating(int rating)
		{
			return _books.Where(b => b.Reviews.Average(b => b.Rating) == rating);
		}

		/// <summary>
		/// returns a Collection of books with given rating range
		/// </summary>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <returns></returns>
		public IEnumerable<Book> GetBooksByRatingIn(int low, int high)
		{
			return _books.Where(b => b.Reviews.Any() && b.Reviews.Average(r => r.Rating) >= low && b.Reviews.Average(r => r.Rating) <= high);
		}


		/// <summary>
		/// returns all distinct authors that have writen book
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Author> GetAuthors()
		{
			return _books.Select(b => b.Author).Distinct();
		}
		/// <summary>
		/// returns true if list of books contains given book by book id, false otherwise
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Contains(int id)
		{
			Book? book = _books.FirstOrDefault(b => b.Id == id);

			return book == null ? false : _books.Contains(book);
		}
		/// <summary>
		/// returns true if list of books contains given book, false otherwise  
		/// </summary>
		/// <param name="book"></param>
		/// <returns></returns>
		public bool Contains(Book book)
		{
			return _books.Contains(book);
		}

		/// <summary>
		/// Loads book data from a CSV file. Each line in the file should represent one book review.
		/// The CSV file format should follow: Book ID, Title, Genre, Description, Author ID, Author Name, Review ID, Review Content, Rating.
		/// If a book with the same ID and author already exists in the system, the review is added to the existing book rather than creating a new one.
		/// </summary>
		/// <param name="csvFile">The CSV file that contains the book data.</param>

		public void Load(FileInfo csvFile)
		{
			var lines = File.ReadAllLines(csvFile.FullName);

			for (int i = 1; i < lines.Length; i++)
			{
				string[] line = lines[i].Split(',');

				if (line.Length != 9)
				{
					Console.WriteLine($"Invalid entry at line {i + 1}: {lines[i]}");
					continue;
				}

				if (int.TryParse(line[0], out int bookId) &&
					!string.IsNullOrEmpty(line[1]) &&
					!string.IsNullOrEmpty(line[2]) &&
					!string.IsNullOrEmpty(line[3]) &&
					int.TryParse(line[4], out int authorId) &&
					!string.IsNullOrEmpty(line[5]) &&
					int.TryParse(line[6], out int reviewId) &&
					!string.IsNullOrEmpty(line[7]) &&
					int.TryParse(line[8], out int reviewRating))
				{
					string bookTitle = line[1];
					string bookGenre = line[2];
					string bookDescription = line[3];
					string authorName = line[5];

					Author author = configureAuthor(authorId, authorName);

					Book? book = _books.FirstOrDefault(b => b.Id == bookId && b.Author.Id == authorId);

					if (book == null)
					{
						book = new Book(bookId, bookTitle, bookGenre, bookDescription, author);
						try
						{
							AddBook(book);
						}
						catch (Exception ex)
						{

							Console.WriteLine($"Invalid entry at line {i + 1}: {lines[i]}");
							Console.WriteLine($"Exception: {ex.Message}");
							continue;
						}
					}

					string reviewContent = line[7];

					Review review = new Review(reviewId, reviewContent, reviewRating);

					book.AddReview(review);
				}
				else
				{
					Console.WriteLine($"Invalid entry at line {i + 1}: {lines[i]}");
				}
			}
		}



		/// <summary>
		/// returns number of reviews from a book
		/// </summary>
		/// <param name="bookID"></param>
		/// <returns></returns>
		public int ReviewCount(int bookID)
		{
			Book? book = _books.FirstOrDefault(b => b.Id == bookID);
			return book?.Reviews.Count ?? 0;
		}

		/// <summary>
		/// if author with give id already exists then return that author, else return new author
		/// </summary>
		/// <param name="authorId"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private Author configureAuthor(int authorId, string name)
		{
			Author? author = GetAuthors().FirstOrDefault(a => a.Id == authorId);
			if (author == null)
			{
				return new Author(authorId, name);

			}
			return author;
		}

		/// <summary>
		/// saving to a file
		/// </summary>
		/// <param name="csvFile">File path</param>
		public void Save(FileInfo csvFile)
		{
			var lines = new List<string>();
			foreach (var book in _books)
			{
				foreach (var review in book.Reviews)
				{
					lines.Add($"{book.Id},{book.Title},{book.Genre},{book.Description},{book.Author.Id},{book.Author.Name},{review.Id},{review.Content},{review.Rating}");
				}
			}
			File.WriteAllLines(csvFile.FullName, lines);
		}

		/// <summary>
		/// return true if genre is valid, otherwise false
		/// </summary>
		/// <param name="genre"></param>
		/// <returns></returns>
		public bool AllowedGenres(string genre)
		{
			switch (genre)
			{
				case "Fiction":
				case "Mystery":
				case "Sci-Fi":
				case "Romance":
				case "Thriller":
				case "Fantasy":
				case "History":
					return true;
				default:
					return false;
			}
		}
	}
}
