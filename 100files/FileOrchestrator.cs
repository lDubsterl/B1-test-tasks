using Azure.Core.GeoJson;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
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
		public void MergeFilesWithSubstitution(string subString, string mergedDataFilename)
		{
			using var mergedFile = new StreamWriter(File.Create(mergedDataFilename));
			int writtenStrings = 0;
			var path = Path.GetDirectoryName(mergedDataFilename) + "\\";
			for (int i = 0; i < 100; i++)
			{
				var filename = path + (i + 1).ToString() + ".txt";
				var filenameCopy = path + (i + 1).ToString() + ".txtc";
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
			Console.WriteLine($"\n{_stringsAmount - writtenStrings} Строк  удалено");
			_stringsAmount = writtenStrings;
		}
		public void ImportInDatabase(string filenameToImport, string tableName = "StringContent")
		{
			var connectionString = @"Server=(localdb)\mssqllocaldb;Database=testtaskdb;Trusted_Connection=True;";
			using (var dbConnection = new SqlConnection(connectionString))
			{
				var query = $"if OBJECT_ID(N'dbo.{tableName}', N'U') is null" +
					"\r\ncreate table StringContent(" +
					"\r\nId int identity," +
					"\r\n\"Date\" date," +
					"\r\nLatins nvarchar(10)," +
					"\r\nCyrillics nvarchar(10)," +
					"\r\n\"Integer\" int," +
					"\r\n\"Real\" float\r\n)";
				var command = new SqlCommand(query, dbConnection);
				dbConnection.Open();
				command.ExecuteNonQuery();
				dbConnection.Close();
			}
			using var reader = new FileReader(filenameToImport, GetConvertTable(), _stringsAmount);
			using var importer = new SqlBulkCopy(connectionString);

			importer.ColumnMappings.Add(0, 1);
			importer.ColumnMappings.Add(1, 2);
			importer.ColumnMappings.Add(2, 3);
			importer.ColumnMappings.Add(3, 4);
			importer.ColumnMappings.Add(4, 5);

			importer.DestinationTableName = tableName;
			importer.BulkCopyTimeout = 3600;
			importer.WriteToServer(reader);
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
	}
}
