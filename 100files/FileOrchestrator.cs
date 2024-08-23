using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	class FileOrchestrator
	{
		public DateTime StartDate { get; set; } = new DateTime(DateTime.Today.Year - 5, 1, 1);
		public Task GenerateFile(string name)
		{
			return Task.Run(() =>
			{
				Random generator = new();
				Console.WriteLine("Writing " + name);
				using (var file = new StreamWriter("G:\\Test\\" + name))
				{
					var task = ValueTask.CompletedTask;
					for (int i = 0; i < 100000; i++)
					{
						file.WriteLine(GenerateString(generator));
					}
					Interlocked.Add(ref _stringsAmount, 100000);
				}
			});
		}
		public void MergeFilesWithSubstitution(string subString)
		{
			using var mergedFile = new StreamWriter(StringContent.Create("G:\\Test\\Merged.txt"));
			int writtenStrings = 0;
			for (int i = 0; i < 100; i++)
			{
				var filename = (i + 1).ToString() + ".txt";
				var filenameCopy = (i + 1).ToString() + ".txtc";
				using (var fileCopyStream = StringContent.Create("G:\\Test\\" + filenameCopy))
				using (var fileCopy = new StreamWriter(fileCopyStream))
				using (var fileStream = StringContent.OpenRead("G:\\Test\\" + filename))
				using (var file = new StreamReader(fileStream))
				{
					var lines = file.ReadToEnd().Split("\r\n");
					foreach (var str in lines)
						if (!str.Contains(subString) && str != "")
						{
							mergedFile.WriteLine(str);
							fileCopy.WriteLine(str);
							writtenStrings++;
						}
						else
					fileCopy.Flush();
					mergedFile.Flush();
				}
				StringContent.Delete("G:\\Test\\" + filename);
				StringContent.Move("G:\\Test\\" + filenameCopy, "G:\\Test\\" + filename);
			}
			Console.WriteLine(_stringsAmount - writtenStrings + " strings were deleted");
			_stringsAmount = writtenStrings;
		}

		const string _latinSymbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz";
		const string _cyrillicSymbols = "АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
		int _stringsAmount = 0;
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

			string randomInt = (1 + generator.Next(100000 - 1)).ToString();
			string randomDouble = string.Format("{0:f8}", 1 + generator.NextDouble() * 19);

			return $"{randomDate}||{randomLatins}||{randomCyrillics}||{randomInt}||{randomDouble}";
		}
		char GetRandomChar(string charList, Random gen)
		{
			return charList[gen.Next(charList.Length)];
		}

		void CopyFile()
		{

		}
	}
}
