namespace ExcelProcessing.Models
{
	public class FullTable(string bankName, int tableYear, int accountId, decimal inActive, decimal inPassive, decimal debt, decimal credit, decimal outActive, decimal outPassive)
	{
		public string BankName { get; set; } = bankName;
		public int TableYear { get; set; } = tableYear;
		public int AccountId { get; set; } = accountId;
		public decimal InActive { get; set; } = inActive;
		public decimal InPassive { get; set; } = inPassive;
		public decimal Debt { get; set; } = debt;
		public decimal Credit { get; set; } = credit;
		public decimal OutActive { get; set; } = outActive;
		public decimal OutPassive { get; set; } = outPassive;
	}
}
