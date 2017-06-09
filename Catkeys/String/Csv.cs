using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Parses and composes CSV text. Stores CSV table data in memory as a List of string arrays.
	/// CSV is a text format used to store a single table of data in human-readable/editable way.
	/// It is a list of lines (called rows or records) containing one or more values (called fields or cells) separated by a separator character.
	/// There is no strictly defined CSV standard. CsvTable uses these rules:
	///		Fields containg <see cref="Separator"/> characters (default ','), <see cref="Quote"/> characters (default '"') and multiple lines must be enclosed in <see cref="Quote"/> characters. Example: "ab, cd".
	///		Each Quote character in such fields must be escaped (replaced) with two <see cref="Quote"/> characters. Example: "ab ""cd"" ef".
	///		Fields that start or end with ASCII space or tab characters must be enclosed in <see cref="Quote"/> characters, unless <see cref="TrimSpaces"/> is false. Example: " ab ".
	///		Rows in CSV text can have different field count.
	/// </summary>
	[DebuggerStepThrough]
	public class CsvTable
	{
		List<string[]> _a;

		///
		public CsvTable() { _a = new List<string[]>(); }

		/// <summary>
		/// Initializes a new <see cref="CsvTable"/> instance and parses CSV text, the same as <see cref="FromString"/>.
		/// Uses default <see cref="Separator"/>, <see cref="Quote"/> and <see cref="TrimSpaces"/> values (',', '"', true).
		/// </summary>
		/// <param name="csv">CSV text.</param>
		/// <exception cref="CatException">Invalid CSV, eg contains incorrectly enclosed fields.</exception>
		public CsvTable(string csv)
		{
			_Parse(csv);
		}

		/// <summary>
		/// Gets the internal List containing rows as string arrays.
		/// It's not a copy; changing it will change the data of this CsvTable variable.
		/// You can do anything with the List. For example, sort it, find rows containing certain field values, get/set field values directly, add/remove rows directly.
		/// All row arrays have Length equal to <see cref="ColumnCount"/>, and it must remain so; you can change Length, but then need to call <c>ColumnCount=newLength</c>.
		/// </summary>
		/// <example><code>x.Data.Sort((a,b) => string.CompareOrdinal(a[0], b[0]));</code></example>
		public List<string[]> Data { get => _a; }

		/// <summary>
		/// Sets or gets the field separator character used when parsing and composing CSV text.
		/// Initially it is ','.
		/// </summary>
		public char Separator { get; set; } = ',';

		/// <summary>
		/// Sets or gets the quote character used when parsing and composing CSV text.
		/// Initially it is '"'.
		/// </summary>
		public char Quote { get; set; } = '"';

		/// <summary>
		/// Whether to ignore ASCII space and tab characters at the beginning and end of fields when parsing CSV.
		/// Initially true.
		/// </summary>
		public bool TrimSpaces { get; set; } = true;

		/// <summary>
		/// Parses CSV text and stores all data in internal List of string arrays.
		/// Depends on these properties: <see cref="Separator"/> (initially ','), <see cref="Quote"/> (initially '"'), <see cref="TrimSpaces"/> (initially true).
		/// </summary>
		/// <param name="csv">
		/// CSV text.
		///	If rows in CSV text have different field count, the longest row sets the <see cref="ColumnCount"/> property and all row array lenghts; array elements of missing CSV fields will be null.
		/// </param>
		/// <exception cref="CatException">Invalid CSV, eg contains incorrectly enclosed fields.</exception>
		public void FromString(string csv)
		{
			_Parse(csv);
		}

		unsafe void _Parse(string csv)
		{
			_columnCount = 0;
			_a = new List<string[]>();
			if(Empty(csv)) return;

			char sep = Separator, quote = Quote;
			string sQuote1 = null, sQuote2 = null;
			bool trim = TrimSpaces;
			var tempRow = new List<string>(8);

			fixed (char* s0 = csv) {
				char* s = s0, se = s0 + csv.Length;
				int nCol = 0;

				for(; s < se; s++) {
					//Read a field.
					string field = "";
					if(trim) { //ltrim
						while(*s == ' ' || *s == '\t') if(++s == se) goto g1;
					}
					char* f, e; //field beginning and end, not including spaces
					bool hasEscapedQuote = false;
					if(*s == quote) {
						f = ++s;
						bool hasClosingQuote = false;
						while(s < se) {
							if(*s++ == quote) {
								if(s < se && *s == quote) { s++; hasEscapedQuote = true; } //escaped quote
								else { hasClosingQuote = true; break; }
							}
						}
						if(!hasClosingQuote) throw new CatException("Invalid CSV: no closing quote.");
						e = s - 1; //before quote
						if(trim) { //rtrim
							while(s < se && (*s == ' ' || *s == '\t')) s++;
						}
						if(s < se && !(*s == sep || *s == '\n')) {
							if(*s == '\r' && s < se + 1 && s[1] == '\n') s++;
							else throw new CatException("Invalid CSV: unescaped quote.");
						}
					} else {
						f = s; //field start
						while(s < se && *s != sep && *s != '\n') s++; //skip field, space and \r
						e = s; if(e > f && *e == '\n' && e[-1] == '\r') e--;
						if(trim) { //rtrim
							while(e > f && (e[-1] == ' ' || e[-1] == '\t')) e--;
						}
					}
					field = new string(f, 0, (int)(e - f)); //field to string
					if(hasEscapedQuote) {
						if(sQuote1 == null) { sQuote1 = new string(quote, 1); sQuote2 = new string(quote, 2); }
						field = field.Replace(sQuote2, sQuote1);
					}
					g1:

					//Print(field);

					tempRow.Add(field);
					if(s >= se || *s == '\n') {
						//Print(_a.Count);
						//Print(tempRow);

						_a.Add(tempRow.ToArray());
						if(tempRow.Count > nCol) nCol = tempRow.Count;
						tempRow.Clear();
					}
				}

				//Make all rows equal length and set _columnCount.
				ColumnCount = nCol;

				//PrintList(RowCount, ColumnCount);
			} //fixed
		}

		/// <summary>
		/// Composes CSV text from the internal List of string arrays.
		/// Depends on these properties: <see cref="Separator"/> (initially ','), <see cref="Quote"/> (initially '"').
		/// </summary>
		public override string ToString()
		{
			if(RowCount == 0 || ColumnCount == 0) return null;

			var s = new StringBuilder();
			char quote = Quote;
			string sQuote1 = null, sQuote2 = null;

			for(int r = 0; r < _a.Count; r++) {
				for(int c = 0; c < _columnCount; c++) {
					var field = _a[r][c];
					if(!Empty(field)) {
						bool hasQuote = field.IndexOf(quote) >= 0;
						if(hasQuote || field.IndexOf(Separator) >= 0 || field[0] == ' ' || field[field.Length - 1] == ' ') {
							if(hasQuote) {
								if(sQuote1 == null) { sQuote1 = new string(quote, 1); sQuote2 = new string(quote, 2); }
								field = field.Replace(sQuote1, sQuote2);
							}
							s.Append(quote);
							s.Append(field);
							s.Append(quote);
						} else s.Append(field);
					}
					if(c < _columnCount - 1) s.Append(Separator);
				}
				s.AppendLine();
			}
			return s.ToString();
		}

		/// <summary>
		/// Gets or sets row count.
		/// The 'get' function simply returns the Count property of the internal List of string arrays.
		/// The 'set' function can add new rows or remove rows at the end.
		/// </summary>
		public int RowCount
		{
			get => _a.Count;
			set
			{
				if(value > _a.Count) {
					string[] row = _columnCount > 0 ? new string[_columnCount] : null;
					_a.Capacity = value;
					while(_a.Count < value) _a.Add(row);
				} else if(value < _a.Count) {
					_a.RemoveRange(value, _a.Count - value);
				}
			}
		}

		/// <summary>
		/// Gets or sets column count.
		/// The 'get' function returns the length of all string arrays in the internal List.
		/// The 'set' function can add new columns or remove columns at the right.
		/// </summary>
		public int ColumnCount
		{
			get => _columnCount;
			set
			{
				if(value <= 0) throw new ArgumentOutOfRangeException();
				//if(value == _columnCount) return;
				for(int r = 0; r < _a.Count; r++) {
					var o = _a[r];
					if(o == null) _a[r] = new string[value];
					else if(o.Length != value) {
						var t = new string[value];
						if(value > _columnCount) o.CopyTo(t, 0);
						else {
							for(int c = 0; c < value; c++) t[c] = o[c];
						}
						_a[r] = t;
					}
				}

				_columnCount = value;
			}
		}
		int _columnCount;

		/// <summary>
		/// Gets or sets a field.
		/// </summary>
		/// <param name="row">0-based row index. With the 'set' function it can be negative or equal to <see cref="RowCount"/>; then adds new row.</param>
		/// <param name="column">0-based column index. With the 'set' function it can be &gt;= <see cref="ColumnCount"/> and &lt; 1000; then makes ColumnCount = column + 1.</param>
		public string this[int row, int column]
		{
			get
			{
				if((uint)column >= _columnCount) throw new ArgumentOutOfRangeException("column");
				if((uint)row >= RowCount) throw new ArgumentOutOfRangeException("row");

				return _a[row][column];
			}
			set
			{
				//Auto-add columns.
				if(column < 0) column = int.MaxValue;
				if(column >= _columnCount) {
					if(column >= 1000) throw new ArgumentOutOfRangeException("column");
					ColumnCount = column + 1;
				}
				//Auto-add row.
				if(row < 0) row = RowCount;
				if(row >= RowCount) {
					if(row > RowCount) throw new ArgumentOutOfRangeException("row");
					_a.Add(new string[_columnCount]);
				}

				_a[row][column] = value;
			}
		}

		/// <summary>
		/// Gets or sets fields in a row.
		/// The 'get' function gets the row array. It's not a copy; changing its elements will change the data of this CsvTable variable.
		/// The 'set' function sets the row array. Does not copy the array, unless its Length is less than <see cref="ColumnCount"/>.
		/// </summary>
		/// <param name="row">0-based row index. With the 'set' function it can be negative or equal to <see cref="RowCount"/>; then adds new row.</param>
		public string[] this[int row]
		{
			get
			{
				if((uint)row >= RowCount) throw new ArgumentOutOfRangeException();

				return _a[row];
			}
			set
			{
				//Auto-add row.
				if(row < 0) row = RowCount;
				if(row >= RowCount) {
					if(row > RowCount) throw new ArgumentOutOfRangeException();
					_a.Add(null);
				}

				var t = value;
				if(value == null || value.Length < _columnCount) {
					//make row length = _columnCount
					t = new string[_columnCount];
					if(value != null) value.CopyTo(t, 0);
				} else if(value.Length > _columnCount) {
					//auto-add columns
					ColumnCount = value.Length;
				}

				_a[row] = t;
			}
		}

		/// <summary>
		/// Inserts new row and sets its fields.
		/// </summary>
		/// <param name="index">0-based row index. If negative or equal to <see cref="RowCount"/>, adds to the end.</param>
		/// <param name="fields">Row fields. Can be a string array or multiple string arguments. Does not copy the array, unless its Length is less than <see cref="ColumnCount"/>. Adds new columns if array Length (or the number of string arguments) is greater than ColumnCount.</param>
		public void InsertRow(int index, params string[] fields)
		{
			if(index < 0) index = RowCount;
			_a.Insert(index, null);
			this[index] = fields;
		}

		/// <summary>
		/// Inserts new empty row.
		/// </summary>
		/// <param name="index">0-based row index. If negative or equal to <see cref="RowCount"/>, adds to the end.</param>
		public void InsertRow(int index)
		{
			InsertRow(index, null);
		}

		/// <summary>
		/// Removes one or more rows.
		/// </summary>
		/// <param name="index">0-based row index.</param>
		/// <param name="count">How many rows to remove, default 1.</param>
		public void RemoveRow(int index, int count = 1)
		{
			_a.RemoveRange(index, count);
		}

		//FUTURE: implement these. Rarely used. Quite much code.
		//Don't need to implement many others because users can call our _a (the Data property) methods directly.
		//public void MoveRow(int from, int to)
		//{

		//}

		//public void InsertColumn(int index)
		//{

		//}

		//public void RemoveColumn(int index)
		//{

		//}

		/// <summary>
		/// Loads and parses a CSV file.
		/// Calls <see cref="File.ReadAllText(string)"/> and <see cref="FromString"/>.
		/// </summary>
		/// <param name="file"></param>
		public void FromFile(string file)
		{
			FromString(File.ReadAllText(file));
		}

		/// <summary>
		/// Composes CSV and saves to file.
		/// Calls <see cref="ToString"/> and <see cref="File.WriteAllText(string, string)"/>.
		/// </summary>
		public void ToFile(string file)
		{
			File.WriteAllText(file, ToString());
			//FUTURE: flags: append, safe, safe+backup
		}

		/// <summary>
		/// Sets an int number field.
		/// </summary>
		/// <param name="row"><see cref="this[int, int]"/></param>
		/// <param name="column"><see cref="this[int, int]"/></param>
		/// <param name="value">The number.</param>
		/// <param name="hex">Let the number be in hexadecimal format, like 0x3A.</param>
		public void SetInt(int row, int column, int value, bool hex = false)
		{
			this[row, column] = hex ? "0x" + value.ToString("X") : value.ToString();
		}

		/// <summary>
		/// Gets an int number field.
		/// </summary>
		/// <param name="row"><see cref="this[int, int]"/></param>
		/// <param name="column"><see cref="this[int, int]"/></param>
		public int GetInt(int row, int column)
		{
			return this[row, column].ToInt32_();
		}

		/// <summary>
		/// Sets a double number field.
		/// </summary>
		/// <param name="row"><see cref="this[int, int]"/></param>
		/// <param name="column"><see cref="this[int, int]"/></param>
		/// <param name="value">The number.</param>
		public void SetDouble(int row, int column, double value)
		{
			this[row, column] = value.ToString_();
		}

		/// <summary>
		/// Gets a double number field.
		/// </summary>
		/// <param name="row"><see cref="this[int, int]"/></param>
		/// <param name="column"><see cref="this[int, int]"/></param>
		public double GetDouble(int row, int column)
		{
			return this[row, column].ToDouble_();
		}
	}
}
