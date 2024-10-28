using Microsoft.EntityFrameworkCore;
using Library_MVC.Models;

namespace Library_MVC.Data
{
	public class MusicLibDBContext : DbContext
	{
		public DbSet<SongModel> Songs { get; set; }

		public MusicLibDBContext(DbContextOptions<MusicLibDBContext> options) : base(options)
		{

		}

	}
}
