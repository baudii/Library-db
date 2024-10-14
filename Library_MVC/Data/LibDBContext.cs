using Microsoft.EntityFrameworkCore;
using Library_MVC.Models;

namespace Library_MVC.Data
{
	public class LibDBContext : DbContext
	{
		public DbSet<BookModel> Books { get; set; }

		private readonly IConfiguration _configuration;

		public LibDBContext(DbContextOptions<LibDBContext> options, IConfiguration configuration) : base(options)
		{
			_configuration = configuration;
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var connectionString = _configuration.GetConnectionString("LibraryDB");
				optionsBuilder.UseNpgsql(connectionString);
			}
		}
	}
}
