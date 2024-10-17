using Microsoft.EntityFrameworkCore;
using Library_MVC.Models;

namespace Library_MVC.Data
{
	public class LibDBContext : DbContext
	{
		public DbSet<BookModel> Books { get; set; }

		public LibDBContext(DbContextOptions<LibDBContext> options, IConfiguration configuration) : base(options)
		{
		}
	}
}
