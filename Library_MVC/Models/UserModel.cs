namespace Library_MVC.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		public string? Email { get; set; }
		public string? PasswordHash { get; set; }
		public string? Salt { get; set; }
	}
}
