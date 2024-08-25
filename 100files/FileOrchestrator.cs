using Azure.Core.GeoJson;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
				using (var file = new StreamWriter(name))
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
			using var mergedFile = new StreamWriter(File.Create("Merged.txt"));
			int writtenStrings = 0;
			for (int i = 0; i < 100; i++)
			{
				var filename = (i + 1).ToString() + ".txt";
				var filenameCopy = (i + 1).ToString() + ".txtc";
				using (var fileCopyStream = File.Create(filenameCopy))
				using (var fileCopy = new StreamWriter(fileCopyStream))
				using (var fileStream = File.OpenRead(filename))
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
				File.Delete(filename);
				File.Move(filenameCopy, filename);
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

		public async Task ImportInDatabase()
		{
			using var mergedFile = new StreamReader(File.OpenRead("Merged.txt"));
			int importedStrings = 0;
			StringContent stringContent;
			var list = new List<StringContent>();
			int stringsBlock = 100000;
			Stopwatch sw = new();
			sw.Start();
			var line = "";
			using (DatabaseContext db = new DatabaseContext(true)) { };
			while (line != null)
			{
				line = mergedFile.ReadLine();
				if (line == "" || line == null) break;
				var parsedLine = line.Split("||");
				stringContent = new StringContent
				{
					Date = DateOnly.Parse(parsedLine[0]),
					Latins = parsedLine[1],
					Cyrillics = parsedLine[2],
					Integer = int.Parse(parsedLine[3]),
					Real = double.Parse(parsedLine[4])
				};
				//db.Add(stringContent);
				importedStrings++;
				list.Add(stringContent);
				if (importedStrings % stringsBlock == 0)
				{
					using (var db = new DatabaseContext(false))
					{
						db.AddRange(list);
						list.Clear();
						db.SaveChanges();
					}
					Console.WriteLine(importedStrings + " strings was imported");
				}
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds / 1000.0);
		}
	}
}
