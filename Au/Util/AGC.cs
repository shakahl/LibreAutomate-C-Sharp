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
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// <see cref="GC"/> extensions.
	/// </summary>
	public static class AGC
	{
		static ConditionalWeakTable<object, _Remover> s_table = new ConditionalWeakTable<object, _Remover>();

		/// <summary>
		/// Calls <see cref="GC.AddMemoryPressure"/>. Later, when object <i>obj</i> is garbage-collected, will call <see cref="GC.RemoveMemoryPressure"/>.
		/// </summary>
		/// <param name="obj">An object of any type.</param>
		/// <param name="size">Unmanaged memory size. It is passed to <b>GC.AddMemoryPressure</b> and <b>GC.RemoveMemoryPressure</b>.</param>
		public static void AddObjectMemoryPressure(object obj, int size)
		{
			GC.AddMemoryPressure(size);
			s_table.Add(obj, new _Remover(size));
		}

		class _Remover
		{
			int _size;

			public _Remover(int size)
			{
				_size = size;
			}

			~_Remover()
			{
				//AOutput.Write("removed " + _size);
				GC.RemoveMemoryPressure(_size);
			}
		}
	}
}
