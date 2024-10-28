namespace Library_MVC.Models
{
	public class SongModel
	{
		public int Id { get; set; }
		public string Author { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Genre { get; set; } = string.Empty;
		public int PublishedYear { get; set; }
		
	}
}
