using System.ComponentModel.DataAnnotations.Schema;

namespace ExcelProcessing.Models
{
	[Table("Turnovers")]
	public class Turnovers
	{
		public int Id { get; set; }
		public required string BankName { get; set; }
		public int TableYear { get; set; }
		public int AccountId { get; set; }
		public decimal Debt {  get; set; }
		public decimal Credit { get; set; }
		public required string Filename { get; set; }
		public int IncomeSaldoId { get; set; }
		public required IncomeSaldo IncomeSaldo { get; set; }
	}
}
