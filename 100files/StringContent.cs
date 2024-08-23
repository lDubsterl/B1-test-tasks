using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	internal class StringContent
	{
		public int Id {  get; set; }
		public DateOnly Date {  get; set; }
		public required string Latins { get; set; }
		public required string Cyrillics { get; set; }
		public int Integer { get; set; }
		public double Real { get; set; }
	}
}
