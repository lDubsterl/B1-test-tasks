using ExcelProcessing.Models;
using IronXL;

//using IronXL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text;
namespace ExcelProcessing.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ExcelProcessingController : ControllerBase
	{
		ApplicationContext _db;
		public ExcelProcessingController(ApplicationContext db)
		{
			_db = db;
		}
		[HttpPost]
		public IActionResult ParseXls(IFormFile[] files)
		{
			foreach (var file in files)
			{
				using (var stream = new FileStream(file.FileName, FileMode.Create))
				{
					file.CopyTo(stream);
				}
				ImportData(file.FileName);  // ������ ������ �� ����� � ��
			}
			return StatusCode(StatusCodes.Status200OK, GetStoringFilesNames()); // ������� ������ ������, ���������� � ��
		}

		[HttpGet]
		public List<string> GetStoringFilesNames()
		{
			return _db.Outcome.Select(row => row.Filename).Distinct().ToList(); // ���������� ���������� ���� ������ � ��
		}

		string _lastRequestedFileName = ""; // ��� ���������������� ����� ��� ��������� �������

		[HttpGet]
		public ObjectResult ExportDbInfo(string fileName)
		{
			if (!System.IO.File.Exists(_lastRequestedFileName) && _lastRequestedFileName != "")
				System.IO.File.Delete(_lastRequestedFileName); // �������� ���������������� ����� ��� ��������� �������� �� �������� �������
			_lastRequestedFileName = "wwwroot\\" + fileName;
			var info = _db.Outcome
				.Where(outSaldo => outSaldo.Filename == fileName)
				.Include(outSaldo => outSaldo.Turnovers)
				.ThenInclude(turnovers => turnovers!.IncomeSaldo)
				.OrderBy(key => key.AccountId)
				.Select(el =>
					new FullTable(el.BankName, el.TableYear, el.AccountId, el.Turnovers!.IncomeSaldo!.Active, el.Turnovers!.IncomeSaldo!.Passive,
					el.Turnovers!.Debt, el.Turnovers!.Credit, el.Active, el.Passive))
				.ToList(); // ���������� ������ �� ��
			if (info.Count > 0)
			{
				var startAccount = info[0].AccountId / 100; // ��������� ������ ������
				decimal[] subAccountSums = new decimal[6]; // ����� �� ������
				decimal[,] classSums = new decimal[9, 6]; // ����� �� ������

				int[] classSumsPositions = new int[9]; // ������� � �������, ���� ����� �������� ����� �� ������
				var ClassNumber = 1;
				var tableSize = info.Count;
				for (int i = 0; i < tableSize; i++)
				{
					if (info[i].AccountId / 100 != startAccount) // ��� �������� � ���� ������ ������
					{
						var isNextClass = false;
						info.Insert(i, new FullTable(info[i].BankName, info[i].TableYear, startAccount,
						subAccountSums[0], subAccountSums[1], subAccountSums[2], subAccountSums[3], subAccountSums[4], subAccountSums[5])); // ������� ������ ������
						if (info[i + 1].AccountId / 1000 != ClassNumber)
						{
							isNextClass = true;
							classSumsPositions[ClassNumber - 1] = i + 1; // ������ ������� ��������� ������ � ������ 
						}

						for (int j = 0; j < subAccountSums.Length; j++)
						{
							classSums[ClassNumber - 1, j] += subAccountSums[j]; // ���������� ����� �� ������
							subAccountSums[j] = 0; // ��������� ����� �� ������
						}
						if (isNextClass) // ������� � ���� �����
							ClassNumber++;
						startAccount = info[i + 1].AccountId / 100; // ��������� ������� ������
						tableSize++;
						continue;
					}
					subAccountSums[0] += info[i].InActive; // ����� �� ������
					subAccountSums[1] += info[i].InPassive;
					subAccountSums[2] += info[i].Debt;
					subAccountSums[3] += info[i].Credit;
					subAccountSums[4] += info[i].OutActive;
					subAccountSums[5] += info[i].OutPassive;
				}
				var bankName = info[0].BankName;
				var tableYear = info[0].TableYear;
				var lastId = info.Last().AccountId;
				info.Add(new FullTable(bankName, tableYear, lastId / 100,
					subAccountSums[0], subAccountSums[1], subAccountSums[2], subAccountSums[3], subAccountSums[4], subAccountSums[5])); // ���������� ��������� ������
				classSumsPositions[^1] = info.Count;
				for (int i = 0; i < subAccountSums.Length; i++)
					classSums[8, i] += subAccountSums[i]; // ���������� ����� ���������� ������
				for (int i = 0; i < ClassNumber; i++)
				{
					info.Insert(classSumsPositions[i] + i, new FullTable(info[i].BankName, info[i].TableYear, -1,
						classSums[i, 0], classSums[i, 1], classSums[i, 2], classSums[i, 3], classSums[i, 4], classSums[i, 5])); // ������� ���� �� ������ � �������
				}
				var balance = new decimal[6];
				for (int i = 0; i < 9; i++)
					for (int j = 0; j < 6; j++)
						balance[j] += classSums[i, j]; // ������� ��������� �������
				info.Add(new FullTable(bankName, tableYear, -2,
					balance[0], balance[1], balance[2], balance[3], balance[4], balance[5])); // ���������� ��������� ������� � �������
				Task.Run(() => ExportInXls(info, fileName)); // ������ ������ � xls ����
			}
			return StatusCode(StatusCodes.Status200OK, info);
		}
		void ExportInXls(List<FullTable> info, string filename)
		{
			var workBook = WorkBook.Create(ExcelFileFormat.XLS);
			var workSheet = workBook.CreateWorkSheet("���� 1"); // �������� xls �����
			workSheet.Merge("A2:G2"); // ����������� ����� � �������
			workSheet["A2"].Value = "��������� ��������� �� ���������� ������"; // ������ �������� � �������
			workSheet.Merge("A3:G3");
			workSheet["A3"].Value = $"�� ������ � 01.01.{info[0].TableYear} �� 31.12.{info[0].TableYear}";
			workSheet.Merge("A4:G4");
			workSheet["A4"].Value = $"�� ����� \"{info[0].BankName}\"";
			workSheet.Merge("A7:A8");
			workSheet["A7"].Value = "�/��";
			workSheet.Merge("B7:C7");
			workSheet["B7"].Value = "�������� ������";
			workSheet.Merge("D7:E7");
			workSheet["D7"].Value = "�������";
			workSheet.Merge("F7:G7");
			workSheet["F7"].Value = "��������� ������";
			workSheet["B8"].Value = "�����";
			workSheet["C8"].Value = "������";
			workSheet["D8"].Value = "�����";
			workSheet["E8"].Value = "������";
			workSheet["F8"].Value = "�����";
			workSheet["G8"].Value = "������";
			string columns = "BCDEFG";
			string[] classes =
				[
		"�����  1  �������� ��������, ����������� ������� � ������������� ��������",
		"�����  2  ��������� � ���� �������� �������� � ���������",
		"�����  3  ����� �� ��������� ��������",
		"�����  4  ������ ������",
		"�����  5  ������������ ���������� �������� � �������� ����� ����������� ���, �������� �������� � ������ ���������",
		"�����  6  ������ ������ � ������ �������",
		"�����  7  ����������� ������� �����",
		"�����  8  ������ �����",
		"�����  9  ������� �����"
		];

			int rowCoordinate = 9; // ������, � ������� ���������� ������ ������
			var accountClass = 0; // ���������� �����
			foreach (var row in info)
			{
				if ((accountClass != row.AccountId / 1000) && (row.AccountId >= 1000)) // ���������� ������ ������� ��� �������� � ����� �����
				{
					var coordinate = $"A{rowCoordinate}:G{rowCoordinate}";
					workSheet.Merge(coordinate);
					workSheet[$"A{rowCoordinate}"].Value = classes[accountClass++]; // ���������� ������ �������� ������
					rowCoordinate++;
				}
				workSheet[$"A{rowCoordinate}"].Value = row.AccountId;

				if (row.AccountId == -1)
					workSheet[$"A{rowCoordinate}"].Value = "�� ������";
				if (row.AccountId == -2)

					workSheet[$"A{rowCoordinate}"].Value = "������";
				workSheet[$"B{rowCoordinate}"].Value = row.InActive; // ������ ������ � �������
				workSheet[$"C{rowCoordinate}"].Value = row.InPassive;
				workSheet[$"D{rowCoordinate}"].Value = row.Debt;
				workSheet[$"E{rowCoordinate}"].Value = row.Credit;
				workSheet[$"F{rowCoordinate}"].Value = row.OutActive;
				workSheet[$"G{rowCoordinate}"].Value = row.OutPassive;

				for (int i = 0; i < 6; i++)
				{
					workSheet[$"{columns[i]}{rowCoordinate}"].FormatString = "0.0000"; // �������������� � �������� ������
					workSheet.AutoSizeColumn(i);
				}

				rowCoordinate++;
			}
			var newName = filename.Replace(".xls", "d.xls");
			workBook.SaveAs(newName); // ������ ����� �� ���� � ���������� �������
			if (!System.IO.File.Exists("wwwroot\\" + filename))
				System.IO.File.Move(newName, "wwwroot\\" + filename);
		}

		Func<string, object>[] GetConvertTable() // ����������� �� �������� � 1 ��������
		{
			var convertTable = new Func<string, object>[7];
			convertTable[0] = str => str;
			convertTable[1] = num => int.Parse(num);
			convertTable[2] = num => int.Parse(num);
			convertTable[3] = num => double.Parse(num);
			convertTable[4] = num => double.Parse(num);
			convertTable[5] = str => str;
			convertTable[6] = num => int.Parse(num);
			return convertTable;
		}
		void ImportData(string filepath) // ������ � �� �� �������� � 1 ��������
		{
			var connectionString = @"Server=(localdb)\mssqllocaldb;Database=testtaskdb;Trusted_Connection=True;";
			string[] tables = ["Income_Saldo", "Turnovers", "Outcome_Saldo"];
			for (int i = 1; i < 4; i++)
			{

				using var reader = new ExcelReader(filepath, GetConvertTable(), i);
				reader.FieldCount = 6;
				using var importer = new SqlBulkCopy(connectionString);
				if (i > 1)
				{
					reader.FieldCount = 7;
					importer.ColumnMappings.Add(6, 7);
				}
				importer.ColumnMappings.Add(0, 1);
				importer.ColumnMappings.Add(1, 2);
				importer.ColumnMappings.Add(2, 3);
				importer.ColumnMappings.Add(3, 4);
				importer.ColumnMappings.Add(4, 5);
				importer.ColumnMappings.Add(5, 6);

				importer.DestinationTableName = tables[i - 1]; // ������� ��� ������
				importer.BulkCopyTimeout = 3600;
				importer.WriteToServer(reader);
			}
		}
	}
}