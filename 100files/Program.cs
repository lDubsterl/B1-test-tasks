using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks.Dataflow;

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
						file.WriteLine(GenerateString(generator));
				}
			});
		}

		public void MergeFilesWithSubstitution(string subString)
		{
			using var mergedFile = new StreamWriter(File.Create("G:\\Test\\Merged.txt"));
			for (int i = 0; i < 100; i++)
			{
				var filename = (i + 1).ToString() + ".txt";
				var filenameCopy = (i + 1).ToString() + ".txtc";

				using (var fileCopyStream = File.Create("G:\\Test\\" + filenameCopy))
				using (var fileCopy = new StreamWriter(fileCopyStream))
				using (var fileStream = File.OpenRead("G:\\Test\\" + filename))
				using (var file = new StreamReader(fileStream))
				{
					var text = file.ReadToEnd();
					var lines = text.Split("\r\n");
					foreach (var str in lines)
						if (!str.Contains(subString))
						{
							mergedFile.WriteLine(str);
							fileCopy.WriteLine(str);
						}
					fileCopy.Flush();
					mergedFile.Flush();
				}
				File.Delete("G:\\Test\\" + filename);
				File.Move("G:\\Test\\" + filenameCopy, "G:\\Test\\" + filename);
			}
		}

		const string _latinSymbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnPpQqRrSsTtUuVvWwXxYyZz";
		const string _cyrillicSymbols = "АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
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
	internal class Program
	{
		static void Main(string[] args)
		{
			var fileOrchestrator = new FileOrchestrator();
			var files = new Task[100];
			var time = new Stopwatch();
			time.Restart();
			for (int i = 0; i < 100; i++)
				files[i] = fileOrchestrator.GenerateFile((i + 1).ToString() + ".txt");
			Task.WaitAll(files);
			time.Stop();
			Console.WriteLine(time.ElapsedMilliseconds / 1000.0);
			time.Restart();
			fileOrchestrator.MergeFilesWithSubstitution("abc");
			time.Stop();
			Console.WriteLine(time.ElapsedMilliseconds / 1000.0);
		}
	}
}
