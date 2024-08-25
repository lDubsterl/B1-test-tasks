using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	internal class DatabaseContext : DbContext
	{
		public DbSet<StringContent> stringContents => Set<StringContent>();
		public DatabaseContext(bool isNew)
		{
			if (isNew)
			{
				Database.EnsureDeleted();
				Database.EnsureCreated();
				ChangeTracker.AutoDetectChangesEnabled = false;
			}
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=testtaskdb;Trusted_Connection=True;");
		}
	}
}
