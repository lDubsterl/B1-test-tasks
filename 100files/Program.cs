using System.Text;

namespace _100files
{
	class FileOrchestrator
	{
		const string _latinSymbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz";
		const string _cyrillicSymbols = "АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
		public DateTime StartDate { get; set; } = new DateTime(DateTime.Today.Year - 5, 1, 1);

		public void GenerateFile(string name)
		{
			Random generator = new();
			Console.WriteLine("Writing " + name);
			using (var file = new FileStream(name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				var task = ValueTask.CompletedTask;
				for (int i = 0; i < 100000; i++)
				{
					byte[] str = Encoding.UTF8.GetBytes(GenerateString(generator));
					file.Write(str);
				}
			}
		}
		string GenerateString(Random generator)
		{
			var randomLatins = new StringBuilder();
			for (int i = 0; i < 10; i++)
				randomLatins.Append(GetRandomChar(_latinSymbols, generator));

			var randomCyrillics = new StringBuilder();
			for (int i = 0; i < 10; i++)
				randomCyrillics.Append(GetRandomChar(_cyrillicSymbols, generator));

			int dateRange = (DateTime.Today - StartDate).Days;
			string randomDate = DateOnly.FromDateTime(StartDate.AddDays(generator.Next(dateRange))).ToString();

			string randomInt = (1 + generator.Next(100 * 10 ^ 6 - 1)).ToString();
			string randomDouble = string.Format("{0:f8}", 1 + generator.NextDouble() * 19);

			return $"{randomDate}||{randomLatins}||{randomCyrillics}||{randomInt}||{randomDouble}";
		}
		char GetRandomChar(string charList, Random gen)
		{
			return charList[gen.Next(charList.Length)];
		}
	}
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
			var fileOrchestrator = new FileOrchestrator();
			for (int i = 0; i < 100; i++)
			{
				fileOrchestrator.GenerateFile((i + 1).ToString() + ".txt");
			}
		}
	}
}
