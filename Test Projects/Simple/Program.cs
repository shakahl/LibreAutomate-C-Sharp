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
//using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Simple
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			//Print(Directory.GetCurrentDirectory());
			//Console.WriteLine(Console.OutputEncoding);
			Console.OutputEncoding = Encoding.UTF8;
			//Console.OutputEncoding = Encoding.GetEncoding(1257); //baltic
			for(int i = 0; i < 3; i++) {
				Console.WriteLine($"{i}. out č ɛ");
				Console.Error.WriteLine($"{i}. err č ɛ");
				//Console.WriteLine($"{i}. out č");
				//Console.Error.WriteLine($"{i}. err č");
			}

			//Console.WriteLine(GetThreadCount());

			//Console.ReadKey();
		}

		internal struct THREADENTRY32
		{
			public int dwSize;
			public uint cntUsage;
			public uint th32ThreadID;
			public uint th32OwnerProcessID;
			public int tpBasePri;
			public int tpDeltaPri;
			public uint dwFlags;
		}
		[DllImport("kernel32.dll")]
		internal static extern bool Thread32First(IntPtr hSnapshot, ref THREADENTRY32 lpte);
		[DllImport("kernel32.dll")]
		internal static extern bool Thread32Next(IntPtr hSnapshot, ref THREADENTRY32 lpte);
		[DllImport("kernel32.dll")]
		internal static extern uint GetCurrentProcessId();
		[DllImport("kernel32.dll")]
		internal static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);
		internal const uint TH32CS_SNAPTHREAD = 0x4;

		static int GetThreadCount()
		{
			var pid = GetCurrentProcessId();
			int n = 0;
			var h = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
			var t = new THREADENTRY32(); t.dwSize = Marshal.SizeOf<THREADENTRY32>();
			for(bool ok = Thread32First(h, ref t); ok; ok = Thread32Next(h, ref t)) {
				if(t.th32OwnerProcessID == pid) n++;
			}
			return n;
		}

		//static Au.POINT _mmm;
		static Au.AuScriptOptions _opt=new Au.AuScriptOptions(null);


		//		[DebuggerStepThrough]
		//		//[Serializable]
		//		public unsafe struct LPARAM //:IXmlSerializable
		//		{
		//#pragma warning disable 1591 //XML doc
		//			//[NonSerialized]
		//			void* _v; //Not IntPtr, because it throws exception on overflow when casting from uint etc.

		//			LPARAM(void* v) { _v = v; }

		//			//LPARAM = int etc
		//			public static implicit operator LPARAM(void* x) { return new LPARAM(x); }
		//			public static implicit operator LPARAM(IntPtr x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(UIntPtr x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(int x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(uint x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(sbyte x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(byte x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(short x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(ushort x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(char x) { return new LPARAM((void*)(ushort)x); }
		//			public static implicit operator LPARAM(long x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(ulong x) { return new LPARAM((void*)x); }
		//			public static implicit operator LPARAM(bool x) { return new LPARAM((void*)(x ? 1 : 0)); }
		//			//public static implicit operator LPARAM(Enum x) { return new LPARAM((void*)(int)x); } //error
		//			//public static implicit operator LPARAM(WPARAM x) { return new LPARAM(x); }
		//			//int etc = LPARAM
		//			public static implicit operator void* (LPARAM x) { return x._v; }
		//			public static implicit operator IntPtr(LPARAM x) { return (IntPtr)x._v; }
		//			public static implicit operator UIntPtr(LPARAM x) { return (UIntPtr)x._v; }
		//			public static implicit operator int(LPARAM x) { return (int)x._v; }
		//			public static implicit operator uint(LPARAM x) { return (uint)x._v; }
		//			public static implicit operator sbyte(LPARAM x) { return (sbyte)x._v; }
		//			public static implicit operator byte(LPARAM x) { return (byte)x._v; }
		//			public static implicit operator short(LPARAM x) { return (short)x._v; }
		//			public static implicit operator ushort(LPARAM x) { return (ushort)x._v; }
		//			public static implicit operator char(LPARAM x) { return (char)(ushort)x._v; }
		//			public static implicit operator long(LPARAM x) { return (long)x._v; }
		//			public static implicit operator ulong(LPARAM x) { return (ulong)x._v; }
		//			public static implicit operator bool(LPARAM x) { return x._v != null; }

		//			public static bool operator ==(LPARAM a, LPARAM b) { return a._v == b._v; }
		//			public static bool operator !=(LPARAM a, LPARAM b) { return a._v != b._v; }

		//			public override string ToString() { return ((IntPtr)_v).ToString(); }

		//			//ISerializable implementation.
		//			//Need it because default serialization: 1. Gets only public members. 2. Exception if void*. 3. If would work, would format like <...><_v>value</_v></...>, but we need <...>value</...>.
		//			//Rejected, because it loads System.Xml.dll and 2 more dlls. Rarely used.
		//			//public XmlSchema GetSchema() { return null; }
		//			//public void ReadXml(XmlReader reader) { _v = (void*)reader.ReadElementContentAsLong(); }
		//			//public void WriteXml(XmlWriter writer) { writer.WriteValue((long)_v); }
		//#pragma warning restore 1591 //XML doc
		//		}
	}
}
