using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	internal class FileReader : AbstractReader
	{

		public override int FieldCount
		{
			get
			{
				return 5;
			}
		}

		int _counter = 0;
		int _generalStringsAmount;
		public FileReader(string filepath, Func<string, object>[] convertTable, int generalStringsAmount) : base(filepath, convertTable)
		{
			_generalStringsAmount = generalStringsAmount;
		}

		public override bool Read()
		{
			_currentLine = _stream.ReadLine()!;
			if (_currentLine == null)
				return false;
			_currentLineValues = _currentLine.Split("||");
			_counter++;
			if (_counter % 100000 == 0)
			{
				Console.WriteLine($"Импортировано {_counter} строк\t|\t{_generalStringsAmount - _counter} строк осталось");
			}
			return true;
		}
	}
}
