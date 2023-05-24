
using BookManagementSystemLibrary;
using System;
using System.Security.Cryptography.X509Certificates;

namespace BookManagementSystemLibrary.ViewerConsoleApp
{
	class Program
	{
		
		static BookService bookService = new BookService();

		static void Main(string[] args)
		{
			string command;

			do
			{
				Console.WriteLine("Enter a command: \n");
				Console.WriteLine("\tadd book");
				Console.WriteLine("\tremove book");
				Console.WriteLine("\tupdate book");
				Console.WriteLine("\tlists book");
				Console.WriteLine("\tlist authors");
				Console.WriteLine("\tshow statistics");
				Console.WriteLine("\tsearch by");
				Console.WriteLine("\tedit");
				Console.WriteLine("\treviews");

				command = Console.ReadLine();

				switch (command)
				{
					case "add book":
						AddBook();
						break;
					case "remove book":
						RemoveBook();
						break;
					case "update book":
						UpdateBook();
						break;
					case "list books":
						ListBooks();
						break;
					case "list authors":
						ListAuthors();
						break;
					case "show statistics":
						ShowStatistics();
						break;
					case "search by":
						SearchBy();
						break;
					case "edit":
						Edit();
						break;
					case "reviews":
						Reviews();
						break;
					default:
						Console.WriteLine("Unknown command.");
						break;
				}

			} while (!string.IsNullOrWhiteSpace(command));

		}

		static void AddBook()
		{
			Console.Write("Enter book ID: ");
			int id = int.Parse(Console.ReadLine());

			Console.Write("Enter book description: ");
			string description = Console.ReadLine();

			Console.Write("Enter book genre: ");
			string genre = Console.ReadLine();

			Console.Write("Enter book title: ");
			string title = Console.ReadLine();

			Console.Write("Enter author ID: ");
			Author? author = configureAuthor();


			bookService.AddBook(id, description, genre, title, author, null);
		}

		static void RemoveBook()
		{
			Console.Write("Enter book ID: ");
			int id = int.Parse(Console.ReadLine());

			bookService.DeleteBook(id);
		}

		static void UpdateBook()
		{
			Console.Write("Enter book ID: ");
			int id = int.Parse(Console.ReadLine());

			var book = bookService.SearchBook(id);
			if (book != null)
			{
				Console.Write("Enter new book title: ");
				book.Title = Console.ReadLine();
				bookService.UpdateBook(book);
			}
		}

		static void ListBooks()
		{
			foreach (var book in bookService)
			{
				Console.WriteLine($"ID: {book.Id}, Title: {book.Title}");
			}
		}

		static void ListAuthors()
		{
			foreach (var author in bookService.GetAuthors())
			{
				Console.WriteLine($"ID: {author.Id}, Name: {author.Name}");
			}
		}


		static void ShowStatistics()
		{
			double averageRating = bookService
				.SelectMany(b => b.Reviews)
				.Average(r => r.Rating);

			double averageBooksPerAuthor = bookService
				.GroupBy(b => b.Author)
				.Average(g => g.Count());

			double averageBooksPerGenre = bookService
				.GroupBy(b => b.Genre)
				.Average(g => g.Count());

			Console.WriteLine($"Average rating: {averageRating}");
			Console.WriteLine($"Average number of books per author: {averageBooksPerAuthor}");
			Console.WriteLine($"Average number of books per genre: {averageBooksPerGenre}");
		}

		static void SearchBy()
		{
			Console.Write("Search by (1- author, 2- genre, 3- rating): ");
			int choice = int.Parse(Console.ReadLine());
			Console.Write("Enter query: ");
			string query = Console.ReadLine();

			switch (choice)
			{
				case 1:
					foreach (var book in bookService.SearchByAuthor(query))
					{
						Console.WriteLine($"ID: {book.Id}, Title: {book.Title}");
					}
					break;
				case 2:
					foreach (var book in bookService.SearchByGenre(query))
					{
						Console.WriteLine($"ID: {book.Id}, Title: {book.Title}");
					}
					break;
				case 3:
					foreach (var book in bookService.SearchByRating(query))
					{
						Console.WriteLine($"ID: {book.Id}, Title: {book.Title}");
					}
					break;
				default:
					Console.WriteLine("Invalid choice.");
					break;
			}
		}

		static void Edit()
		{
			Console.WriteLine("Enter book ID to edit: ");
			int id = int.Parse(Console.ReadLine());

			Console.WriteLine($"You chose to edit {bookService[id]}");

			var book = bookService.SearchBook(id);
			if (book != null)
			{
				Console.WriteLine("What would you like to edit?");
				Console.WriteLine("1 - Title");
				Console.WriteLine("2 - Description");
				Console.WriteLine("3 - Genre");
				Console.WriteLine("4 - Author");

				int choice = int.Parse(Console.ReadLine());
				switch (choice)
				{
					case 4:
						book.Author = configureAuthor();
						break;
					default:
						Console.WriteLine("Invalid choice.");
						break;
				}
				bookService.UpdateBook(book);
			}
			else
			{
				Console.WriteLine("No book found with that ID.");
			}
		}

		private static Author? configureAuthor()
		{
			Console.WriteLine("Enter 'n' to create a new author, or 'e' to use an existing author: ");
			char authorChoice = char.Parse(Console.ReadLine());
			Author author = null;
			if (authorChoice == 'n')
			{
				while (true)
				{
					Console.WriteLine("Enter a unique Id for the author: ");
					if (int.TryParse(Console.ReadLine(), out int authorId))
					{
						if (!bookService.GetAuthors().Any(author => author.Id == authorId))
						{
							Console.WriteLine("Enter author name: ");
							string name = Console.ReadLine();
							author = new Author(authorId, name);
							break;
						}
						else
						{
							Console.WriteLine("This Id is already in use. Please enter a unique Id.");
						}
					}
					else
					{
						Console.WriteLine("Invalid input. Please enter a valid number for the author Id.");
					}
				}

			}
			else if (authorChoice == 'e')
			{
				while (author == null)
				{
					Console.WriteLine("Enter author ID: ");
					int authorId = int.Parse(Console.ReadLine());
					author = bookService.GetAuthors().FirstOrDefault(a => a.Id == authorId);
					if (author == null)
					{
						Console.WriteLine("Incorrect id");
					}
				}
				
			}
			return author;
		}

		static void Reviews()
		{
			Console.Write("Enter book ID: ");
			int id = int.Parse(Console.ReadLine());

			var book = bookService.SearchBook(id);
			if (book != null)
			{
				Console.Write("Write a review: ");
				string reviewContent = Console.ReadLine();
				Console.Write("Enter a rating (1-5): ");
				int rating = int.Parse(Console.ReadLine());
				var newReview = new Review(book.Reviews.Count + 1, reviewContent, rating);
				bookService.AddReview(newReview, id);

				Console.WriteLine($"Reviews for {book.Title}:");
				foreach (var review in book.Reviews)
				{
					Console.WriteLine($"ID: {review.Id}, Content: {review.Content}, Rating: {review.Rating}");
				}
			}
		}


	}
}