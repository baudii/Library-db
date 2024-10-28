using Library_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library_MVC.Data
{
	public class AuthDbContext : IdentityDbContext<UserModel>
	{
		public DbSet<UserModel> Users { get; set; }
		//public DbSet<IdentityRole> Roles { get; set; }

		public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
/*
			// Настройка связи между пользователями и ролями
			builder.Entity<UserModel>()
				.HasOne(u => u.Role)
				.WithMany()
				.HasForeignKey(u => u.RoleId);*/
		}
	}
}
