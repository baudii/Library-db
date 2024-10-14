using Microsoft.EntityFrameworkCore;
using Library_MVC.Models;

namespace Library_MVC.Data
{
	public class UserContext : DbContext
	{
		public DbSet<UserModel> Users { get; set; }
		public UserContext(DbContextOptions<UserContext> options) : base(options)
		{
			//Database.EnsureCreated();
		}
	}
}
