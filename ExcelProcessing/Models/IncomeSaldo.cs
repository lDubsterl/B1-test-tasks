using System.ComponentModel.DataAnnotations.Schema;

namespace ExcelProcessing.Models
{
	[Table("Income_Saldo")]
	public class IncomeSaldo
	{
		public int Id { get; set; }
		public required string BankName { get; set; }
		public int TableYear { get; set; }
		public int AccountId { get; set; }
		public decimal Active {  get; set; }
		public decimal Passive { get; set; }
		public required string Filename { get; set; }
	}
}
