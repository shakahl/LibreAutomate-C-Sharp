using System;
using System.Collections.Generic;
using System.Text;
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
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Util
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
			//char* _caseTable; //char[0x10000] containing lower-case versions of the first 0x10000 characters. Now it is in AuCpp dll.
			byte* _base64Ptr;
			byte* _hexPtr;

			fixed byte _base64Arr['z' + 1];
			fixed byte _hexArr[55];

			//internal char* CaseTable()
			//{
			//	if(_caseTable == null) {
			//		var t = (char*)Api.VirtualAlloc(default, 0x20000); //faster than NativeHeap.Alloc when need big memory, especially when need to zero it
			//		if(t == null) throw new OutOfMemoryException();
			//		for(int i = 0; i < 0x10000; i++) t[i] = (char)i;
			//		Api.CharLowerBuff(t, 0x10000);
			//		if(_caseTable == null) _caseTable = t; else Api.VirtualFree((IntPtr)t); //another thread can outrun us
			//	} //speed: 350
			//	return _caseTable;
			//}

			internal byte* Base64Table()
			{
				if(_base64Ptr == null) {
					fixed (byte* t = _base64Arr) {
						for(uint i = 0; i <= 'z'; i++) t[i] = _DecodeChar(i);
						_base64Ptr = t;
					}
				}
				return _base64Ptr;

				byte _DecodeChar(uint ch)
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
			}

			internal byte* HexTable()
			{
				if(_hexPtr == null) {
					fixed (byte* t = _hexArr) {
						for(int u = 0; u < 55; u++) {
							char c = (char)(u + '0');
							if(c >= '0' && c <= '9') t[u] = (byte)u;
							else if(c >= 'A' && c <= 'F') t[u] = (byte)(c - ('A' - 10));
							else if(c >= 'a' && c <= 'f') t[u] = (byte)(c - ('a' - 10));
							else t[u] = 0xFF;
						}
						_hexPtr = t;
					}
				}
				return _hexPtr;
			}
		}

		/// <summary>
		/// Gets native-memory char[0x10000] containing lower-case versions of the first 0x10000 characters.
		/// Auto-creates when called first time in process. The memory is shared by appdomains.
		/// </summary>
		internal static char* LowerCase
		{
			get { var v = _lcTable; if(v == null) _lcTable = v = Cpp.Cpp_LowercaseTable(); return v; } //why operator ?? cannot be used with pointers?
		}
		static char* _lcTable;
		//never mind: this library does not support ucase/lcase chars 0x10000-0x100000 (surrogate pairs).
		//	Tested with IsUpper/IsLower: about 600 such chars exist. ToUpper/ToLower can convert 40 of them. Equals/StartsWith/IndexOf/etc fail.

		/// <summary>
		/// Gets table for <see cref="AConvert.Base64Decode(char*, int, void*, int)"/> and co.
		/// Auto-creates when called first time in process. The memory is shared by appdomains.
		/// </summary>
		internal static byte* Base64
		{
			get { var v = _Base64Table; if(v == null) _Base64Table = v = LibProcessMemory.Ptr->tables.Base64Table(); return v; }
		}
		static byte* _Base64Table;

		/// <summary>
		/// Gets table for <see cref="AConvert.HexDecode(string, void*, int, int)"/> and co.
		/// Auto-creates when called first time in process. The memory is shared by appdomains.
		/// </summary>
		internal static byte* Hex
		{
			get { var v = _HexTable; if(v == null) _HexTable = v = LibProcessMemory.Ptr->tables.HexTable(); return v; }
		}
		static byte* _HexTable;
	}
}
