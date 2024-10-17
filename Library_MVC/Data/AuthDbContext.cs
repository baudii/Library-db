using Library_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library_MVC.Data
{
	public class AuthDbContext : IdentityDbContext<UserModel>
	{
		public AuthDbContext(DbContextOptions options) : base(options)
		{
		}
	}
}
