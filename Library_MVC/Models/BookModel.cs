namespace Library_MVC.Models
{
	public class BookModel
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		public string Genre { get; set; } = string.Empty;
		
		public int? AuthorId { get; set; }
		public AuthorModel? Author {  get; set; }
	}
}
