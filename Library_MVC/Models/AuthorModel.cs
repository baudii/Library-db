namespace Library_MVC.Models
{
	public class AuthorModel
	{
		public int Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }

		public List<BookModel> Books { get; set; } = new();
	}
}
