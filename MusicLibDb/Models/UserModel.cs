using Microsoft.AspNetCore.Identity;

namespace Library_MVC.Models
{
	public class UserModel : IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
/*		public string? RoleId { get; set; }
		public IdentityRole? Role { get; set; }*/
	}
}
