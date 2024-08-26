using System.Diagnostics;
using System.Xml.Serialization;

namespace _100files
{
	internal class Program
	{
		static async Task PrintProgressMessage(string msg, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				Console.Clear();
				Console.SetCursorPosition(0, 0);
				Console.Write(msg);
				await Task.Delay(500);
				Console.Write(".");
				await Task.Delay(500);
				Console.Write(".");
				await Task.Delay(500);
				Console.Write(".");
				await Task.Delay(500);
			}
		}

		static bool IsDirectoryExists(string filename)
		{
			if (!Path.Exists(Path.GetDirectoryName(filename)))
			{
				Console.WriteLine("Заданный путь не найден");
				return false;
			}
			return true;
		}
		static async Task Main()
		{
			Console.Write("Введите путь для сохранения сгенерированных файлов: ");
			string? path = Console.ReadLine() + "\\";
			if (!Path.Exists(path))
			{
				Console.WriteLine("Заданный путь не найден");
				return;
			}
			Console.Clear();
			var tokenSource = new CancellationTokenSource();
			var console = Task.Run(() => PrintProgressMessage("Идёт генерация файлов", tokenSource.Token));

			var fileOrchestrator = new FileOrchestrator();
			var files = new Task[100];

			for (int i = 0; i < 100; i++)
			{
				var filename = path + (i + 1).ToString() + ".txt";
				files[i] = fileOrchestrator.GenerateFile(filename);
			}
			Task.WaitAll(files);
			tokenSource.Cancel();
			await console;
			string? choice;
			do
			{
				Console.WriteLine("\n1. Объединить файлы с удалением подстроки\n2. Импортировать файл в базу данных\n0. Выход\n");
				choice = Console.ReadLine();
				if (choice == "1")
				{
					Console.Write("Введите подстроку для удаления: ");
					string? subString = Console.ReadLine();
					Console.Write("\nВведите путь к файлу, в который объединить данные: ");
					string? filename = Console.ReadLine();

					if (!IsDirectoryExists(filename!))
						continue;

					tokenSource = new CancellationTokenSource();
					console = Task.Run(() => PrintProgressMessage("Идёт объединение файлов", tokenSource.Token));
					fileOrchestrator.MergeFilesWithSubstitution(subString!, filename!);
					tokenSource.Cancel();
				}
				if (choice == "2")
				{
					Console.Write("\nВведите путь к файлу,который нужно импортировать: ");
					string? filename = Console.ReadLine();
					if (!IsDirectoryExists(filename!))
						continue;
					fileOrchestrator.ImportInDatabase(filename!);
				}
			} while (choice != "0");
		}
	}
}
