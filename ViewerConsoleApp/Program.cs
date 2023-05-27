
using BookManagementSystemLibrary;
using System;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;

namespace BookManagementSystemLibrary.ViewerConsoleApp
{
	class Program
	{
		// An instance of BookService that performs operations on books.
		static BookService bookService = new BookService();
		static void Main(string[] args)
		{
			FileInfo csvFile = new(@"..\..\..\..\books.csv");
			bookService.Load(csvFile);
			while (true)
			{
				string? command;

				do
				{
					Console.WriteLine("Enter a command:");
					Console.WriteLine("\tadd book");
					Console.WriteLine("\tremove book");
					Console.WriteLine("\tlist books");
					Console.WriteLine("\tlist authors");
					Console.WriteLine("\tshow statistics");
					Console.WriteLine("\tsearch by");
					Console.WriteLine("\tedit");
					Console.WriteLine("\treviews");
					Console.WriteLine("exit");


					command = Console.ReadLine();

					switch (command)
					{
						case "add book":
							AddBook();
							break;
						case "remove book":
							RemoveBook();
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
						case "exit":
							Console.WriteLine("Exiting application");
							return; 
						default:
							Console.WriteLine("Unknown command.");
							break;
					}

				} while (true);
			}

		}

		// Add a book to the collection. The user will be prompted to enter details of the book.
		static void AddBook()
		{
			try
			{
				Console.Write("Enter book ID: ");
				if (!int.TryParse(Console.ReadLine(), out int id))
				{
					Console.WriteLine("Invalid input. Please enter a valid number for the book ID.");
					return;
				}
				if (bookService.Contains(id))
				{
					Console.WriteLine("This book ID is already in use. Please enter a unique ID.");
					return;
				}
				Console.Write("Enter book title: ");
				string? title = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(title))
				{
					Console.WriteLine("Invalid input. Please enter a valid title for the book.");
					return;
				}

				Console.Write("Enter book genre: ");

				string? genre = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(genre))
				{
					Console.WriteLine("Invalid input. Please enter a valid genre for the book.");
					return;
				}


				Console.Write("Enter book description: ");
				string? description = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(description))
				{
					Console.WriteLine("Invalid input. Please enter a valid description for the book.");
					return;
				}


				Author? author = configureAuthor();
				if (author == null)
				{
					Console.WriteLine("No author provided or an error occurred while creating/selecting the author.");
					return;
				}

				

				bookService.AddBook(id, description, genre, title, author, null);
				Console.WriteLine($"Book '{title}' added successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An unexpected error occurred: {ex.Message}");
			}
		}

		// Remove a book from the collection. The user will be prompted to enter the ID of the book.

		static void RemoveBook()
		{
			Console.Write("Enter book ID: ");
			if (int.TryParse(Console.ReadLine(), out int id))
			{
				bookService.DeleteBook(id);
			}
			else
			{
				Console.WriteLine("Error occurred. Invalid input.");
			}
		}

		// List all the books in the collection.

		static void ListBooks()
		{
			foreach (var book in bookService)
			{
				Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Genre: {book.Genre}");
			}
		}

		// List all the authors.
		static void ListAuthors()
		{
			foreach (var author in bookService.GetAuthors())
			{
				Console.WriteLine($"ID: {author.Id}, Name: {author.Name}");
			}
		}

		// Show some statistics about the books in the collection.

		static void ShowStatistics()
		{
			double averageRating = bookService
				.SelectMany(b => b.Reviews)
				.Average(r => r.Rating);

			int averageBooksPerAuthor = (int)bookService
				.GroupBy(b => b.Author)
				.Average(g => g.Count());

			int averageBooksPerGenre = (int)bookService
				.GroupBy(b => b.Genre)
				.Average(g => g.Count());

			Console.WriteLine($"Average rating: {averageRating.ToString("0.00")}");
			Console.WriteLine($"Average number of books per author: {averageBooksPerAuthor}");
			Console.WriteLine($"Average number of books per genre: {averageBooksPerGenre}");
		}

		// Search for books by author, genre or rating. The user will be prompted to enter the search criteria.

		static void SearchBy()
		{
			Console.Write("Search by (1- author, 2- genre, 3- rating): ");
			int.TryParse(Console.ReadLine(), out int choice);
			Console.Write("Enter query: ");
			string? query = Console.ReadLine();
			query ??= "";

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

		// Edit the details of a book. The user will be prompted to enter the ID of the book and the new details.

		static void Edit()
		{
			int id;
			while (true) {
				Console.WriteLine("Enter book ID to edit: ");
				if (int.TryParse(Console.ReadLine(), out  id)) break;
				else Console.WriteLine("Invalid input !");
			}
			
			Console.WriteLine($"You chose to edit {bookService[id]}");

			var book = bookService.SearchBook(id);
			if (book != null)
			{
				Console.WriteLine("What would you like to edit?");
				Console.WriteLine("\tTitle");
				Console.WriteLine("\tDescription");
				Console.WriteLine("\tGenre");
				Console.WriteLine("\tAuthor");

				string choice = Console.ReadLine() ?? "";
				switch (choice)
				{
					case "Title":
						Console.WriteLine("new Title: ");
						string? title = Console.ReadLine();
						if(title != null)
						{
							book.Title = title;
						}
						else
						{
							Console.WriteLine("wrong Title!!!");

						}
						break;
					case "Description":
						Console.WriteLine("new Description: ");
						string? description = Console.ReadLine();
						if (description != null)
						{
							book.Description = description;
						}
						else
						{
							Console.WriteLine("wrong description!!!");

						}
						break;
					case "Genre":
						Console.WriteLine("new Genre: ");
						string? genre = Console.ReadLine();
						if (genre != null && bookService.AllowedGenres(genre))
						{
							book.Genre = genre;
						}
						else
						{
							Console.WriteLine("wrong Genre!!!");

						}
						break;
					case "Author":
						Author? author = configureAuthor();
						if (author != null)
						{
							book.Author = author;
						}
						else
						{
							Console.WriteLine("wrong Author!!!");

						}
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

		// Configure the author for a book. The user can choose to create a new author or use an existing one.

		private static Author? configureAuthor()
		{
			try
			{
				Console.WriteLine("Enter 'n' to create a new author, or 'e' to use an existing author: ");
				char authorChoice;
				if (!char.TryParse(Console.ReadLine(), out authorChoice))
				{
					Console.WriteLine("Invalid input. Please enter 'n' or 'e'.");
					return null;
				}

				Author? author = null;
				switch (authorChoice)
				{
					case 'n':
						while (true)
						{
							Console.WriteLine("Enter a unique Id for the author: ");
							if (int.TryParse(Console.ReadLine(), out int authorId))
							{
								if (!bookService.GetAuthors().Any(author => author.Id == authorId))
								{
									Console.WriteLine("Enter author name: ");
									string? name = Console.ReadLine();
									if (string.IsNullOrWhiteSpace(name))
									{
										Console.WriteLine("Invalid name. Please enter a valid name.");
										continue;
									}
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
						break;
					case 'e':
						while (author == null)
						{
							Console.WriteLine("Enter author ID: ");
							if (int.TryParse(Console.ReadLine(), out int authorId))
							{
								author = bookService.GetAuthors().FirstOrDefault(a => a.Id == authorId);
								if (author == null)
								{
									Console.WriteLine("Incorrect id. No author found with the given ID.");
								}
							}
							else
							{
								Console.WriteLine("Invalid input. Please enter a valid number for the author Id.");
							}
						}
						break;
					default:
						Console.WriteLine("Invalid choice. Please enter 'n' to create a new author or 'e' to use an existing one.");
						return null;
				}

				return author;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An unexpected error occurred: {ex.Message}");
				return null;
			}
		}

		// Manage reviews for a book. The user can choose to see existing reviews or add a new one.

		static void Reviews()
		{
			try
			{
				Console.Write("Enter book ID: ");

				if (!int.TryParse(Console.ReadLine() , out int id))
				{
					Console.WriteLine("Invalid input. Please enter a number.");
					return;
				}
				var book = bookService.SearchBook(id);
				if (book == null)
				{
					Console.WriteLine($"No book found with the ID: {id}");
					return;
				}

				Console.Write("See reviews[1] or add a review[2] ?: ");

				if (!int.TryParse(Console.ReadLine(), out int option) && (option != 1 || option != 2))
				{
					Console.WriteLine("Invalid input. Please enter a number.");
					return;
				}

				if(option == 1)
				{
					Console.WriteLine($"Reviews for {book.Title}:");
					foreach (var review in book.Reviews)
					{
						Console.WriteLine($"ID: {review.Id}, Content: {review.Content}, Rating: {review.Rating}");
					}
				}
				if (option == 2)
				{
					Console.WriteLine("Write a review: ");
					string? reviewContent = Console.ReadLine();
					if (string.IsNullOrWhiteSpace(reviewContent))
					{
						Console.WriteLine("Invalid review content.");
						return;
					}

					Console.WriteLine("Enter a rating (1-5): ");
					int rating;
					if (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
					{
						Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
						return;
					}

					var newReview = new Review(book.Reviews.Count + 1, reviewContent, rating);
					bookService.AddReview(newReview, id);
				}

			
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An unexpected error occurred: {ex.Message}");
			}
		}
	}
}