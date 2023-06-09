namespace BookManagementSystemLibrary.Tests
{
	public class BookServiceTests
	{
		private BookService _bookService;
		[SetUp]
		public void Setup()
		{
			_bookService = new BookService();
		}

		[Test]
		public void AddBook_ShouldIncreaseCount()
		{
			Author author = new Author(1, "test Author");

			Book book = new Book(1, "Test Book", "Fiction", "Test Description", author);


			_bookService.AddBook(book);

			
			Assert.That(_bookService.BookCount, Is.EqualTo(1));
		}

		[Test]
		public void AddBook_ShouldNotAddDuplicateBook()
		{
			Author author = new Author(1, "test Author");

			Book book1 = new Book(1, "Test Book", "Fiction", "Test Description", author);
			Book book2 = new Book(1, "Test Book", "Fiction", "Test Description", author);


			_bookService.AddBook(book1);

			var exception = Assert.Throws<ArgumentException>(() => _bookService.AddBook(book2));
			Assert.That(exception.Message, Is.EqualTo("book with given Id already exists"));

		}

		[Test]
		public void AddBook_ShouldAddMultipleBooks()
		{
			Author author = new Author(1, "test Author");

			Book book1 = new Book(1, "Test Book", "Fiction", "Test Description", author);
			Book book2 = new Book(2, "Test Book", "Fiction", "Test Description", author);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);

			Assert.That(_bookService.BookCount, Is.EqualTo(2));
		}
		[Test]
		public void AddReview_ShouldntAddDuplicateReport()
		{
			Author author = new Author(1, "test Author");

			Book book1 = new Book(1, "Test Book", "Fiction", "Test Description", author);
			_bookService.AddBook(book1);

			Review review1 = new Review(1, "Test Review 2", 3);
			Review review2 = new Review(1, "Test Review 2", 3);

			_bookService.AddReview(review1, book1.Id);

			Assert.IsFalse(_bookService.AddReview(review2, book1.Id));
		}

		[Test]
		public void AddReview_ShouldAddReview()
		{
			Author author = new Author(1, "test Author");
			Book book1 = new Book(1, "Test Book", "Fiction", "Test Description", author);

			Review review = new Review(1, "Test Report", 3);
			_bookService.AddBook(book1);
			_bookService.AddReview(review, book1.Id);

			Assert.That(_bookService.ReviewCount(book1.Id), Is.EqualTo(1));
		}

		[Test]
		public void Load_WrongFileName()
		{
			FileInfo csvFile = new FileInfo("nonexistent.csv");

			Assert.Throws<FileNotFoundException>(() => _bookService.Load(csvFile));
		}

		
		[Test]
		public void SearchBook_ByAuthor_ShouldReturnMatchingBooks()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Fiction", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			string[] result = _bookService[author1];

			Assert.That(result.Length, Is.EqualTo(2));
			CollectionAssert.Contains(result, book1.Title);
			CollectionAssert.Contains(result, book3.Title);
		}

		[Test]
		public void SearchBook_ByGenre_ShouldReturnMatchingBooks()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Mystery", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			var result = _bookService.SearchByGenre("Fiction");

			Assert.That(result.Count(), Is.EqualTo(2));
			CollectionAssert.Contains(result, book1);
			CollectionAssert.Contains(result, book3);
		}

		[Test]
		public void SearchBook_ByRating_ShouldReturnMatchingBooks()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Fiction", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			Review review1 = new Review(1, "Review 1", 4);
			Review review2 = new Review(2, "Review 2", 5);
			Review review3 = new Review(3, "Review 3", 3);

			book1.AddReview(review1);
			book2.AddReview(review2);
			book3.AddReview(review3);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			var result = _bookService.SearchByRating("4");

			Assert.That(result.Count(), Is.EqualTo(2));
			CollectionAssert.Contains(result, book1);
			CollectionAssert.Contains(result, book2);
		}

		[Test]
		public void GetAuthors_ShouldReturnAllAuthors()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");
			Author author3 = new Author(3, "Author 3");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Fiction", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author3);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);


			var result = _bookService.GetAuthors();

			Assert.That(result.Count(), Is.EqualTo(3));
			CollectionAssert.Contains(result, author1);
			CollectionAssert.Contains(result, author2);
			CollectionAssert.Contains(result, author3);
		}
		[Test]
		public void AllowedGenres_ShouldReturnTrueForValidGenres()
		{
			// Test code here
			Assert.IsTrue(_bookService.AllowedGenres("Fiction"));
			Assert.IsTrue(_bookService.AllowedGenres("Mystery"));
			Assert.IsTrue(_bookService.AllowedGenres("Sci-Fi"));
			Assert.IsTrue(_bookService.AllowedGenres("Romance"));
			Assert.IsTrue(_bookService.AllowedGenres("Thriller"));
			Assert.IsTrue(_bookService.AllowedGenres("Fantasy"));
		}

		[Test]
		public void AllowedGenres_ShouldReturnFalseForInvalidGenres()
		{
			Assert.IsFalse(_bookService.AllowedGenres("InvalidGenre"));
			Assert.IsFalse(_bookService.AllowedGenres("1234"));
			Assert.IsFalse(_bookService.AllowedGenres("Test"));
		}

		[Test]
		public void DeleteBook_ShouldRemoveBook()
		{
			Author author = new Author(1, "test Author");
			Book book = new Book(1, "Test Book", "Fiction", "Test Description", author);

			_bookService.AddBook(book);
			_bookService.DeleteBook(book.Id);

			Assert.That(_bookService.BookCount, Is.EqualTo(0));
			Assert.IsFalse(_bookService.Contains(book.Id));
		}

		[Test]
		public void UpdateBook_ShouldUpdateExistingBook()
		{
			Author author = new Author(1, "test Author");
			Book book = new Book(1, "Test Book", "Fiction", "Test Description", author);
			Book updatedBook = new Book(1, "Updated Book", "Mystery", "Updated Description", author);

			_bookService.AddBook(book);
			_bookService.UpdateBook(updatedBook);

			Book retrievedBook = _bookService.SearchBook(book.Id);
			Assert.That(retrievedBook.Title, Is.EqualTo(updatedBook.Title));
			Assert.That(retrievedBook.Genre, Is.EqualTo(updatedBook.Genre));
			Assert.That(retrievedBook.Description, Is.EqualTo(updatedBook.Description));
			Assert.That(retrievedBook.Author, Is.EqualTo(updatedBook.Author));
		}

		[Test]
		public void UpdateBook_ShouldThrowExceptionForNonExistingBook()
		{
			Author author = new Author(1, "test Author");
			Book book = new Book(1, "Test Book", "Fiction", "Test Description", author);

			Assert.Throws<ArgumentException>(() => _bookService.UpdateBook(book));
		}

		[Test]
		public void GetBooksByAuthorID_ShouldReturnMatchingBooks()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Fiction", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			var result = _bookService.GetBooksByAuthorID(author1.Id);

			Assert.That(result.Count(), Is.EqualTo(2));
			CollectionAssert.Contains(result, book1);
			CollectionAssert.Contains(result, book3);
		}

		[Test]
		public void GetBooksByGenre_ShouldReturnMatchingBooks()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Mystery", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			var result = _bookService.GetBooksByGenre("Fiction");

			Assert.That(result.Count(), Is.EqualTo(2));
			CollectionAssert.Contains(result, book1);
			CollectionAssert.Contains(result, book3);
		}

		[Test]
		public void GetBooksByRating_ShouldReturnMatchingBooks()
		{
			// Arrange
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Fiction", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			Review review1 = new Review(1, "Review 1", 4);
			Review review2 = new Review(2, "Review 2", 5);
			Review review3 = new Review(3, "Review 3", 4);

			book1.AddReview(review1);
			book2.AddReview(review2);
			book3.AddReview(review3);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			var result = _bookService.GetBooksByRating(4);

			CollectionAssert.AreEquivalent(result.Select(b => b.Title), new[] { book1.Title, book3.Title });
		}

		[Test]
		public void GetBooksByRatingIn_ShouldReturnMatchingBooks()
		{
			Author author1 = new Author(1, "Author 1");
			Author author2 = new Author(2, "Author 2");

			Book book1 = new Book(1, "Book 1", "Fiction", "Description 1", author1);
			Book book2 = new Book(2, "Book 2", "Fiction", "Description 2", author2);
			Book book3 = new Book(3, "Book 3", "Fiction", "Description 3", author1);

			Review review1 = new Review(1, "Review 1", 4);
			Review review2 = new Review(2, "Review 2", 5);
			Review review3 = new Review(3, "Review 3", 3);

			book1.AddReview(review1);
			book2.AddReview(review2);
			book3.AddReview(review3);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);
			_bookService.AddBook(book3);

			var result = _bookService.GetBooksByRatingIn(3, 4);

			Assert.That(result.Count(), Is.EqualTo(2));
			CollectionAssert.Contains(result, book1);
			CollectionAssert.Contains(result, book3);
		}

		[Test]
		public void GetReviewsOrNull_ShouldReturnReviewsForExistingBook()
		{
			Author author = new Author(1, "Author 1");
			Book book = new Book(1, "Book 1", "Fiction", "Description 1", author);

			Review review1 = new Review(1, "Review 1", 4);
			Review review2 = new Review(2, "Review 2", 5);
			book.AddReview(review1);
			book.AddReview(review2);

			_bookService.AddBook(book);

			var result = _bookService.GetReviewsOrNull(book.Id);

			Assert.IsNotNull(result);
			Assert.That(result.Count, Is.EqualTo(2));
			CollectionAssert.Contains(result, review1);
			CollectionAssert.Contains(result, review2);
		}

		[Test]
		public void GetReviewsOrNull_ShouldReturnNullForNonExistingBook()
		{
			var result = _bookService.GetReviewsOrNull(1);

			Assert.IsNull(result);
		}
	}
}