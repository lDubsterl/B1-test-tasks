using System.Diagnostics;

namespace _100files
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var db = new DatabaseContext(true);
			db.Dispose();
			var fileOrchestrator = new FileOrchestrator();
			var files = new Task[100];
			var time = new Stopwatch();
			//time.Restart();
			//for (int i = 0; i < 100; i++)
			//	files[i] = fileOrchestrator.GenerateFile((i + 1).ToString() + ".txt");
			//Task.WaitAll(files);
			//time.Stop();
			//Console.WriteLine(time.ElapsedMilliseconds / 1000.0);
			//time.Restart();
			//fileOrchestrator.MergeFilesWithSubstitution("abc");
			//time.Stop();
			//Console.WriteLine(time.ElapsedMilliseconds / 1000.0);
			fileOrchestrator.ImportInDatabase();
		}
	}
}
