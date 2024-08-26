using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	internal class FileReader : IDataReader
	{
		public int FieldCount
		{
			get
			{
				return 5;
			}
		}

		readonly StreamReader _stream;

		string[]? _currentLineValues;
		string? _currentLine;

		static int _counter = 0;
		int _generalStringsAmount;

		readonly Func<string, object>[] _convertTable;
		int IDataReader.Depth => throw new NotImplementedException();

		bool IDataReader.IsClosed => throw new NotImplementedException();

		int IDataReader.RecordsAffected => throw new NotImplementedException();

		object IDataRecord.this[string name] => throw new NotImplementedException();

		object IDataRecord.this[int i] => throw new NotImplementedException();

		public FileReader(string filepath, Func<string, object>[] convertTable, int generalStringsAmount)
		{
			_convertTable = convertTable;
			_stream = new StreamReader(filepath);
			_currentLine = null;
			_currentLineValues = null;
			_generalStringsAmount = generalStringsAmount;
		}

		public object GetValue(int i)
		{
			return _convertTable[i](_currentLineValues![i]);
		}

		public bool Read()
		{
			if (_stream.EndOfStream)
			{
				Console.WriteLine($"Imported {_counter} strings");
				Console.WriteLine($"{_generalStringsAmount - _counter} strings left");
				return false;
			}

			_currentLine = _stream.ReadLine()!;
			_currentLineValues = _currentLine.Split("||");
			_counter++;
			if (_counter % 100000 == 0)
			{
				Console.WriteLine($"Imported {_counter} strings");
				Console.WriteLine($"{_generalStringsAmount - _counter} strings left");
			}
			return true;
		}

		public void Dispose()
		{
			_stream.Dispose();
		}

		void IDataReader.Close()
		{
			throw new NotImplementedException();
		}

		DataTable? IDataReader.GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		bool IDataReader.NextResult()
		{
			throw new NotImplementedException();
		}

		bool IDataRecord.GetBoolean(int i)
		{
			throw new NotImplementedException();
		}

		byte IDataRecord.GetByte(int i)
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		char IDataRecord.GetChar(int i)
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		IDataReader IDataRecord.GetData(int i)
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetDataTypeName(int i)
		{
			throw new NotImplementedException();
		}

		DateTime IDataRecord.GetDateTime(int i)
		{
			throw new NotImplementedException();
		}

		decimal IDataRecord.GetDecimal(int i)
		{
			throw new NotImplementedException();
		}

		double IDataRecord.GetDouble(int i)
		{
			throw new NotImplementedException();
		}

		Type IDataRecord.GetFieldType(int i)
		{
			throw new NotImplementedException();
		}

		float IDataRecord.GetFloat(int i)
		{
			throw new NotImplementedException();
		}

		Guid IDataRecord.GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		short IDataRecord.GetInt16(int i)
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetInt32(int i)
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetInt64(int i)
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetName(int i)
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetOrdinal(string name)
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetString(int i)
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		bool IDataRecord.IsDBNull(int i)
		{
			throw new NotImplementedException();
		}
	}
}
