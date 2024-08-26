using Azure.Core.GeoJson;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	class FileOrchestrator
	{
		public DateTime StartDate { get; set; } = new DateTime(DateTime.Today.Year - 5, 1, 1);
		
		const string _latinSymbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz";		
		const string _cyrillicSymbols = "АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
		
		int _stringsAmount = 0;
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

		Func<string, object>[] GetConvertTable()
		{
			var convertTable = new Func<string, object>[5];
			convertTable[0] = str => DateOnly.Parse(str);
			convertTable[1] = str => str;
			convertTable[2] = str => str;
			convertTable[3] = str => int.Parse(str);
			convertTable[4] = str => double.Parse(str);
			return convertTable;
		}

		public void ImportInDatabase()
		{
			var connectionString = @"Server=(localdb)\mssqllocaldb;Database=testtaskdb;Trusted_Connection=True;";
			using var reader = new FileReader("G:\\Test\\Merged.txt", GetConvertTable(), _stringsAmount);

			Stopwatch sw = new();
			sw.Start();
			using var importer = new SqlBulkCopy(connectionString);

			importer.ColumnMappings.Add(0, 1);
			importer.ColumnMappings.Add(1, 2);
			importer.ColumnMappings.Add(2, 3);
			importer.ColumnMappings.Add(3, 4);
			importer.ColumnMappings.Add(4, 5);

			importer.DestinationTableName = "StringContents";
			importer.BulkCopyTimeout = 3600;
			importer.WriteToServer(reader);
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds / 1000.0);
		}
	}
}
