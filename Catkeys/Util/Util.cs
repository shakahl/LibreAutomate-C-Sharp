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
		public static ushort RegWinClassHidden(string name, Api.WNDPROC wndProc)
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
		/// <summary>
		/// Gets the entry assembly of current appdomain.
		/// Normally instead can be used Assembly.GetEntryAssembly(), but it fails if appdomain launched through DoCallBack.
		/// </summary>
		public static Assembly AppdomainAssembly
		{
			get
			{
				if(_appdomainAssembly==null) {
					var asm = Assembly.GetEntryAssembly(); //fails if this domain launched through DoCallBack
					if(asm==null) asm=AppDomain.CurrentDomain.GetAssemblies()[1]; //[0] is mscorlib, 1 should be our assembly
					_appdomainAssembly=asm;
				}
				return _appdomainAssembly;
			}
		}
		static Assembly _appdomainAssembly;

		public static IntPtr GetModuleHandleOf(Type t)
		{
			return t==null ? Zero : Marshal.GetHINSTANCE(t.Module);

			//Tested these to get caller's module without Type parameter:
			//This is dirty/dangerous and 50 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(new StackFrame(1).GetMethod().DeclaringType.Module);
			//This is dirty/dangerous, does not support multi-module assemblies and 12 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().GetLoadedModules()[0]);
			//This is dirty/dangerous/untested and 12 times slower: [MethodImpl(MethodImplOptions.AggressiveInlining)] ... return Marshal.GetHINSTANCE(MethodBase.GetCurrentMethod().DeclaringType.Module);
		}

		public static IntPtr GetModuleHandleOf(Assembly asm)
		{
			return asm==null ? Zero : Marshal.GetHINSTANCE(asm.GetLoadedModules()[0]);
		}

		public static IntPtr GetModuleHandleOfAppdomainEntryAssembly()
		{
			return GetModuleHandleOf(AppdomainAssembly);
		}

		public static void MinimizeMemory()
		{
			//return;
			Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), (UIntPtr)(~0U), (UIntPtr)(~0U));
		}

		public static unsafe int CharPtrLength(char* p)
		{
			if(p==null) return 0;
			for(int i = 0; ; i++) if(*p=='\0') return i;
		}

		public static unsafe int CharPtrLength(char* p, int nMax)
		{
			if(p==null) return 0;
			for(int i = 0; i<nMax; i++) if(*p=='\0') return i;
			return nMax;
		}
	}





	//TEST


	//public static class Path //cannot be Path, because hides System.IO.Path
	//{
	//	public static string Expand(string path) { return "rrrr"; }
	//}

}
