using Microsoft.EntityFrameworkCore;
using Library_MVC.Models;

namespace Library_MVC.Data
{
	public class LibDBContext : DbContext
	{
		public DbSet<BookModel> Books { get; set; }
		public DbSet<AuthorModel> Authors { get; set; }

		public LibDBContext(DbContextOptions<LibDBContext> options) : base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<BookModel>()
						.HasOne(book => book.Author)
						.WithMany(author => author.Books)
						.HasForeignKey(book => book.AuthorId);
		}

	}
}
