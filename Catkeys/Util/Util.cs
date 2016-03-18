using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Catkeys;
using static Catkeys.NoClass;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Util
{
	public static class NoClass
	{
		public static readonly IntPtr NULL = default(IntPtr); //info: IntPtr cannot be const

		public static void Mes(string s)
		{
			//MessageBox.Show(s); //does not have topmost option
			Api.MessageBox(NULL, s, "Test", 0x50000); //MB_TOPMOST|MB_SETFOREGROUND
		}
	}

	public static class Paths
	{
		public static string App
		{
			get { return AppDomain.CurrentDomain.BaseDirectory; }
			//private set;
		}

		public static string CombineApp(string file) { return Path.Combine(App, file); }
	}

	public static class Window
	{
		/// <summary>
		/// Native handle of current process exe file.
		/// </summary>
		public static readonly IntPtr hInstApp = Process.GetCurrentProcess().Handle;
		/// <summary>
		/// Native handle of current module (dll or exe file).
		/// </summary>
		public static readonly IntPtr hInstModule = Marshal.GetHINSTANCE(typeof(Window).Module);

		/// <summary>
		/// Registers native window class that can be used for simple hidden windows.
		/// Returns class atom.
		/// Sets class name, procedure address and current module handle. All other properties 0.
		/// </summary>
		public static ushort RegWinClassHidden(string name, Api.WndProc wndProc)
		{
			var w = new Api.WNDCLASSEX(name, wndProc);
			return Api.RegisterClassEx(ref w);
		}
	}

	public class SharedMemoryFast
	{
		protected IntPtr _hmap, _mem;

		/// <summary>
		/// Pointer to the base of the shared memory.
		/// </summary>
		public IntPtr mem { get { return _mem; } }

		~SharedMemoryFast() { Close(); }

		public bool Create(string name, uint size)
		{
			Close();
			_hmap = Api.CreateFileMapping((IntPtr)(~0), NULL, 4, 0, size, name);
			if(_hmap==NULL) return false;
			_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, UIntPtr.Zero);
			//if(_mem && nbZero) Zero(_mem, nbZero); //don't zero all, because it maps memory for all possibly unused pages. Tested: although MSDN says that the memory is zero, it is not true.
			return _mem!=NULL;
		}

		public bool Open(string name)
		{
			Close();
			_hmap = Api.OpenFileMapping(0x000F001F, false, name);
			if(_hmap==NULL) return false;
			_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, UIntPtr.Zero);
			return _mem!=NULL;
		}

		public void Close()
		{
			if(_mem!=NULL) { Api.UnmapViewOfFile(_mem); _mem=NULL; }
			if(_hmap!=NULL) { Api.CloseHandle(_hmap); _hmap=NULL; }
		}
	}

	public static class Misc
	{
		public static void MinimizeMemory()
		{
			//return;
			Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), (UIntPtr)(~0U), (UIntPtr)(~0U));
		}
	}






	//TEST


	//public static class Path //cannot be Path, because hides System.IO.Path
	//{
	//	public static string Expand(string path) { return "rrrr"; }
	//}

}
