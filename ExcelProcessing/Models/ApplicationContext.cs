﻿using Microsoft.EntityFrameworkCore;

namespace ExcelProcessing.Models
{
	public class ApplicationContext: DbContext
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
		
	}
}
