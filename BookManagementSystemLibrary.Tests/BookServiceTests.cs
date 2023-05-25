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

			
			Assert.AreEqual(1, _bookService.BookCount);
		}

		[Test]
		public void AddBook_ShouldNotAddDuplicateBook()
		{
			Author author = new Author(1, "test Author");

			Book book1 = new Book(1, "Test Book", "Fiction", "Test Description", author);
			Book book2 = new Book(1, "Test Book", "Fiction", "Test Description", author);


			_bookService.AddBook(book1);

			var exception = Assert.Throws<ArgumentException>(() => _bookService.AddBook(book2));
			Assert.AreEqual("book with given Id already exists", exception.Message);

		}

		[Test]
		public void AddBook_ShouldAddMultipleBooks()
		{
			Author author = new Author(1, "test Author");

			Book book1 = new Book(1, "Test Book", "Fiction", "Test Description", author);
			Book book2 = new Book(2, "Test Book", "Fiction", "Test Description", author);

			_bookService.AddBook(book1);
			_bookService.AddBook(book2);

			Assert.AreEqual(2, _bookService.BookCount);
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

			Assert.AreEqual(1, _bookService.ReviewCount(book1.Id));
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

			Assert.AreEqual(2, result.Length);
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

			Assert.AreEqual(2, result.Count());
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

			Assert.AreEqual(2, result.Count());
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

			Assert.AreEqual(3, result.Count());
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
	}
}