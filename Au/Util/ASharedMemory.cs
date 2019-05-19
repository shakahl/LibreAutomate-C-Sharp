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
	/// Memory that can be used by multiple processes and app domains.
	/// Faster and more raw/unsafe than System.IO.MemoryMappedFiles.MemoryMappedFile.
	/// </summary>
	/// <seealso cref="AWnd.More.CopyDataStruct"/>
	//[DebuggerStepThrough]
	public unsafe static class ASharedMemory
	{
		/// <summary>
		/// Creates named shared memory of specified size. Opens if already exists.
		/// Returns shared memory address in this process.
		/// Calls API <msdn>CreateFileMapping</msdn> and API <msdn>MapViewOfFile</msdn>.
		/// </summary>
		/// <param name="name">Shared memory name. Case-insensitive.</param>
		/// <param name="size">Shared memory size. Ignored if the shared memory already exists.</param>
		/// <param name="created">Receives true if created and not opened.</param>
		/// <exception cref="AException">The API failed.</exception>
		/// <remarks>
		/// Once the memory is created, it is alive at least until this process ends. Other processes can keep the memory alive even after that.
		/// There is no Close function to close the native shared memory object handle. The OS closes it when this process ends.
		/// </remarks>
		public static void* CreateOrGet(string name, uint size, out bool created)
		{
			created = false;
			lock("AF2liKVWtEej+lRYCx0scQ") {
				string interDomainVarName = "AF2liKVWtEej+lRYCx0scQ" + name.Lower();
				if(!InterDomainVariables.GetVariable(name, out IntPtr t)) {
					var hm = Api.CreateFileMapping((IntPtr)~0, Api.SECURITY_ATTRIBUTES.ForLowIL, Api.PAGE_READWRITE, 0, size, name);
					if(hm.Is0) goto ge;
					created = ALastError.Code != Api.ERROR_ALREADY_EXISTS;
					t = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
					if(t == default) { hm.Dispose(); goto ge; }
					InterDomainVariables.SetVariable(name, t);
				}
				return (void*)t;
			}
			ge:
			throw new AException(0, "*open shared memory");
		}
	}
}
