
using BookManagementSystemLibrary;
using System;

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
				Console.WriteLine("Enter a command: ");
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
			int authorID = int.Parse(Console.ReadLine());

			bookService.AddBook(id, description, genre, title, null, authorID);
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