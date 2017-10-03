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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys.Util
{
	/// <summary>
	/// Lookup tables for various functions of this library.
	/// Multiple appdomains share the same tables.
	/// </summary>
	//[DebuggerStepThrough]
	internal unsafe static class LibTables
	{
		internal struct ProcessVariables
		{
			char* _caseTable; //char[0x10000] containing lower-case versions of the first 0x10000 characters
			byte* _base64Table;
			byte* _hexTable;

			internal char* CaseTable()
			{
				if(_caseTable == null) {
					var t = (char*)Api.VirtualAlloc(Zero, 0x20000); //faster than NativeHeap.Alloc when need big memory, especially when need to zero it
					if(t == null) throw new OutOfMemoryException();
					for(int i = 0; i < 0x10000; i++) t[i] = (char)i;
					Api.CharLowerBuff(t, 0x10000);
					if(_caseTable == null) _caseTable = t; else Api.VirtualFree((IntPtr)t); //another thread can outrun us
				} //speed: 350
				return _caseTable;
			}

			internal byte* Base64Table()
			{
				if(_base64Table == null) {
					_base64Table = (byte*)NativeHeap.Alloc('z' + 1);
					for(uint i = 0; i <= 'z'; i++) _base64Table[i] = _Base64DecodeChar(i);
				}
				return _base64Table;
			}

			static byte _Base64DecodeChar(uint ch)
			{
				if(ch >= 'A' && ch <= 'Z') return (byte)(ch - 'A' + 0);    // 0 range starts at 'A'
				if(ch >= 'a' && ch <= 'z') return (byte)(ch - 'a' + 26);   // 26 range starts at 'a'
				if(ch >= '0' && ch <= '9') return (byte)(ch - '0' + 52);   // 52 range starts at '0'
				if(ch == '+') return 62;
				if(ch == '/') return 63;
				if(ch == '-') return 62; //alternative for '+' which cannot be used in URLs and XML tag names
				if(ch == '_') return 63; //alternative for '/' which cannot be used in paths, URLs and XML tag names
				return 0xff;
			}

			internal byte* HexTable()
			{
				if(_hexTable == null) {
					_hexTable = (byte*)NativeHeap.Alloc(55);
					for(int u = 0; u < 55; u++) {
						char c = (char)(u + '0');
						if(c >= '0' && c <= '9') _hexTable[u] = (byte)u;
						else if(c >= 'A' && c <= 'F') _hexTable[u] = (byte)(c - ('A' - 10));
						else if(c >= 'a' && c <= 'f') _hexTable[u] = (byte)(c - ('a' - 10));
						else _hexTable[u] = 0xFF;
					}
				}
				return _hexTable;
			}
		}

		//info: this pattern of accessing a table is fastest. Optimized code accesses the table directly,
		//	like 'movzx eax,byte ptr [eax+0E073C0h]', where 0E073C0h is table address and eax is index.
		//	Even if we at first store it in a local variable, which is needed because not in all cases the code will be optimized in such way.

		/// <summary>
		/// Gets native-memory char[0x10000] containing lower-case versions of the first 0x10000 characters.
		/// Auto-creates when called first time. The memory is shared by appdomains.
		/// </summary>
		internal static char* LowerCase { get; } = _LowerCaseTable;
		static char* _LowerCaseTable { get => LibProcessMemory.Ptr->tables.CaseTable(); }

		/// <summary>
		/// Gets table for <see cref="Convert_.Base64Decode(char*, int, void*, int)"/> and co.
		/// Auto-creates when called first time. The memory is shared by appdomains.
		/// </summary>
		internal static byte* Base64 { get; } = _Base64Table;
		static byte* _Base64Table { get => LibProcessMemory.Ptr->tables.Base64Table(); }

		/// <summary>
		/// Gets table for <see cref="Convert_.HexDecode(string, void*, int, int)"/> and co.
		/// Auto-creates when called first time. The memory is shared by appdomains.
		/// </summary>
		internal static byte* Hex { get; } = _HexTable;
		static byte* _HexTable { get => LibProcessMemory.Ptr->tables.HexTable(); }
	}
}
