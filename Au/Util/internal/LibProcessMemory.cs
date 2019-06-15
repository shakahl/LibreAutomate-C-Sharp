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

using Au.Types;
using static Au.AStatic;

namespace Au.Util
{
	/// <summary>
	/// Memory shared by all appdomains of current process.
	/// Size 0x10000 (64 KB). Initially zero.
	/// </summary>
	/// <remarks>
	/// When need to prevent simultaneous access of the memory by multiple threads, use <c>lock("uniqueString"){...}</c> .
	/// It locks in all appdomains, because literal strings are interned, ie shared by all appdomains.
	/// Using some other object with 'lock' would lock only in that appdomain. Maybe except Type.
	/// Use this only in single module, because ngened modules have own interned strings.
	/// </remarks>
	unsafe struct LibProcessMemory
	{
		#region variables used by our library classes
		//Be careful with types whose sizes are different in 32 and 64 bit process. Use long and cast to IntPtr etc.

		//public int test;
		internal LibTables.ProcessVariables tables; //sizeof = 200
		internal LibWorkarounds.ProcessVariables workarounds;
		internal ThreadPoolSTA.ProcessVariables threadPool;
		internal ATRole taskRole;
		//internal AThread.ProcessVariables thread_;
		//internal APerf.Inst perf;

		#endregion

		/// <summary>
		/// Gets pointer to the memory.
		/// </summary>
		internal static LibProcessMemory* Ptr { get; }

		/// <summary>
		/// Memory size.
		/// </summary>
		internal const int Size = 0x10000;

		static LibProcessMemory()
		{
#if true
			Ptr = (LibProcessMemory*)InterDomainVariables.GetVariable("Au_LibProcessMemory", () => Api.VirtualAlloc(default, Size));
#else
			//SHOULDDO: try AuCpp dll. Then can store tables in dll shared memory; then we'll have max 2 copies of it - in 64-bit dll and in 32-bit dll. But slower startup.
			//	Also try shared memory for tables.
#endif
		}
	}
}
