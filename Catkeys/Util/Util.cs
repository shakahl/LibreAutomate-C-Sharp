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
	[DebuggerStepThrough]
	public static class NoClass
	{
	}

	/// <summary>
	/// Enum extension methods.
	/// </summary>
	[DebuggerStepThrough]
	public static class Enum_
	{
		/// <summary>
		/// Returns true if this has one or more flags specified in flagOrFlags.
		/// It is different from HasFlag, which returns true if this has ALL specified flags.
		/// Speed: 0.1 mcs. It is several times slower than HasFlag, and 100 times slower than operators & !=0.
		/// </summary>
		public static bool HasAny(this Enum t, Enum flagOrFlags)
		{
			return (Convert.ToUInt64(t) & Convert.ToUInt64(flagOrFlags)) !=0;
			//info: cannot apply operator & to Enum, or cast to uint, or use unsafe pointer.
		}
		////same speed:
		//public static bool Has<T>(this T t, T flagOrFlags) where T: struct
		//{
		//	//return ((uint)t & (uint)flagOrFlags) !=0; //error
		//	return (Convert.ToUInt64(t) & Convert.ToUInt64(flagOrFlags)) !=0;
		//}

		public static void SetFlag(ref Enum t, Enum flag, bool set)
		{
			ulong _t = Convert.ToUInt64(t), _f = Convert.ToUInt64(flag);
			if(set) _t|=_f; else _t&=~_f;
			t=(Enum)(object)_t; //can do it better?
			//info: Cannot make this a true extension method.
			//	If we use 'this Enum t', t is a copy of the actual variable, because Enum is a value type.
			//	That is why we need ref.
			//Speed not tested.
		}
	}

	[DebuggerStepThrough]
	public static class Paths
	{
		public static string App
		{
			get { return AppDomain.CurrentDomain.BaseDirectory; }
			//private set;
		}

		public static string CombineApp(string file) { return Path.Combine(App, file); }
	}

	[DebuggerStepThrough]
	public static class Window
	{
		///// <summary>
		///// Native handle of current process exe file.
		///// </summary>
		//public static readonly IntPtr hInstApp = Api.GetModuleHandle(null);

		/// <summary>
		/// Native handle of current module (dll or exe file).
		/// </summary>
		public static readonly IntPtr CatkeysModuleHandle = Marshal.GetHINSTANCE(typeof(Window).Module);

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

	//TODO: test SafeMemoryMappedFileHandle, SafeMemoryMappedViewHandle.
	[DebuggerStepThrough]
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
			_hmap = Api.CreateFileMapping((IntPtr)(~0), Zero, 4, 0, size, name);
			if(_hmap==Zero) return false;
			_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
			//if(_mem && nbZero) Zero(_mem, nbZero); //don't zero all, because it maps memory for all possibly unused pages. Tested: although MSDN says that the memory is zero, it is not true.
			return _mem!=Zero;
		}

		public bool Open(string name)
		{
			Close();
			_hmap = Api.OpenFileMapping(0x000F001F, false, name);
			if(_hmap==Zero) return false;
			_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
			return _mem!=Zero;
		}

		public void Close()
		{
			if(_mem!=Zero) { Api.UnmapViewOfFile(_mem); _mem=Zero; }
			if(_hmap!=Zero) { Api.CloseHandle(_hmap); _hmap=Zero; }
		}
	}

	[DebuggerStepThrough]
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
