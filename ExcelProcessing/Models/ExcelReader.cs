using ExcelDataReader;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ExcelProcessing.Models
{
	public class ExcelReader : _100files.AbstractReader // ридер для xls файлов
	{
		public new int FieldCount { get; set; }

		string _bankName;
		string _year;
		int foreignId = 1; //переменная для записи id в качестве внешнего ключа

		Func<string, string[]>? _parseString;
		IExcelDataReader _excelDataReader;
		public ExcelReader(string filepath, Func<string, object>[] convertTable, int tableNumber) : base(filepath, convertTable)
		{
			_excelDataReader = ExcelReaderFactory.CreateReader(_stream.BaseStream); // создание объекта для чтения xls файла

			_excelDataReader.Read(); // чтение очередной строки
			_bankName = _excelDataReader.GetString(0); // получение string из 1 ячейки таблицы

			_excelDataReader.SkipRows(1); // пропуск строк
			_excelDataReader.Read();

			var str = _excelDataReader.GetString(0);
			var regex = new Regex("\\d{4}");
			var match = regex.Match(str); // извлечение года таблицы
			_year = match.Value;

			_excelDataReader.SkipRows(6);
			ChooseTable(tableNumber, filepath); // выбор таблицы для записи в бд
		}

		public override bool Read()
		{
			if (_excelDataReader.Read() == false)
				return false;

			var lineId = ReadRow();
			var line = lineId.Item1;
			var id = lineId.Item2;

			while (id < 1000) // пропуск подводящих итоги строк
			{
				_excelDataReader.Read();
				lineId = ReadRow();
				line = lineId.Item1;
				id = lineId.Item2;
				if (line.ToString().Contains("БАЛАНС")) // при встрече строки с балансом - завершение чтения
					return false;
			}

			_currentLineValues = _parseString!(line.ToString()); // разбор строки на значения, которые запишутся в бд
			foreignId++;
			return true;
		}

		public override void Dispose()
		{
			base.Dispose();
			GC.SuppressFinalize(this);
		}
		(StringBuilder, int) ReadRow()
		{
			int i = 0, accountId = -1;
			var line = new StringBuilder();
			do
			{
				var cell = _excelDataReader.GetValue(i); // получение значения из ячейки
				if (cell == null)
					break;
				if (i == 0)
					int.TryParse(cell.ToString()!, out accountId);
				line.Append(cell + ";"); // формирование строки данных
				i++;
				if (i == _excelDataReader.FieldCount)
					break;
			} while (true);
			return (line, accountId);
		}
		void ChooseTable(int tableNumber, string filename)
		{
			switch (tableNumber) // выбор способа разбора строки данных в зависимости от таблицы
			{
				case 1:
					_parseString = (string line) =>
					{
						var values = line.TrimEnd(';').Split(";", StringSplitOptions.RemoveEmptyEntries);
						return [_bankName, _year, values[0], values[1], values[2], filename];
					};
					break;
				case 2:
					_parseString = (string line) =>
					{
						var values = line.TrimEnd(';').Split(";", StringSplitOptions.RemoveEmptyEntries);
						return [_bankName, _year, values[0], values[3], values[4], filename, foreignId.ToString()];
					};
					break;
				case 3:
					_parseString = (string line) =>
					{
						var values = line.TrimEnd(';').Split(";", StringSplitOptions.RemoveEmptyEntries);
						return [_bankName, _year, values[0], values[5], values[6], filename, foreignId.ToString()];
					};
					break;
				default:
					throw new ArgumentException($"Передано неверное  значение {tableNumber}. Допустимые: 1-3");
			}
		}
	}
	static class IExcelDataReaderExtension
	{
		public static void SkipRows(this IExcelDataReader reader, int rowCount)
		{
			for (int i = 0; i < rowCount; i++)
			{
				reader.Read();
			}
		}
	}
}
