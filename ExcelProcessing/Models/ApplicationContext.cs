using Microsoft.EntityFrameworkCore;

namespace ExcelProcessing.Models
{
	public class ApplicationContext : DbContext // контекст для работы с бд
	{
		public DbSet<IncomeSaldo> Income { get; set; }
		public DbSet<Turnovers> Turnovers { get; set; }
		public DbSet<OutcomeSaldo> Outcome { get; set; }
		public ApplicationContext()
		{
			Database.EnsureCreated();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=testtaskdb;Trusted_Connection=True;");
		}
		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder
					.Properties<decimal>()
					.HavePrecision(19, 4); // установка 4 знаков после запятой для чисел в бд
		}
	}
}
