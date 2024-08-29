using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100files
{
	public abstract class AbstractReader: IDataReader // класс, представляющий ридер данных из файла
	{
		public virtual int FieldCount { get; }

		protected readonly StreamReader _stream;

		protected string[]? _currentLineValues;
		protected string? _currentLine;

		protected readonly Func<string, object>[] _convertTable; // таблица преобразований данных

		int IDataReader.Depth => throw new NotImplementedException();

		bool IDataReader.IsClosed => throw new NotImplementedException();

		int IDataReader.RecordsAffected => throw new NotImplementedException();

		object IDataRecord.this[string name] => throw new NotImplementedException();

		object IDataRecord.this[int i] => throw new NotImplementedException();

		public AbstractReader(string filepath, Func<string, object>[] convertTable)
		{
			_convertTable = convertTable;
			_stream = new StreamReader(filepath);
			_currentLine = null;
			_currentLineValues = null;
		}

		public virtual object GetValue(int i)
		{
			return _convertTable[i](_currentLineValues![i]);
		}

		public abstract bool Read();

		public virtual void Dispose()
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
