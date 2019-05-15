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
using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Provides a cached reusable instance of StringBuilder per thread. It's an optimisation that reduces the number of instances constructed and collected.
	/// Used like <c>using(new Au.Util.LibStringBuilder(out var b)) { b.Append("example"); var s = b.ToString(); }</c>.
	/// </summary>
	/// <remarks>
	/// This is a modified copy of the .NET internal StringBuilderCache class.
	/// The cache uses 2 [ThreadLocal] StringBuilder instances, which allows 1 nesting level. Not error to use deeper nesting level, but then gets new StringBuilder, not from the cache.
	/// </remarks>
	internal struct LibStringBuilder : IDisposable
	{
		StringBuilder _sb;

		/// <summary>
		/// 2000. The cache is not used if capacity is bigger.
		/// </summary>
		public const int MAX_BUILDER_SIZE = 2000;

		[ThreadStatic] private static StringBuilder t_cachedInstance, t_cachedInstance2;

		/// <summary>
		/// Gets a new or cached/cleared StringBuilder of the specified capacity, min 200.
		/// </summary>
		public LibStringBuilder(out StringBuilder sb, int capacity = 200)
		{
			if(capacity <= MAX_BUILDER_SIZE) {
				if(capacity < 200) capacity = 200;
				StringBuilder b = t_cachedInstance;
				bool alt = b == null; if(alt) b = t_cachedInstance2;
				if(b != null) {
					if(capacity <= b.Capacity) {
						if(alt) t_cachedInstance2 = null; else t_cachedInstance = null;
						b.Clear();
						//ADebug.Print("StringBuilder cached, alt=" + alt);
						sb = _sb = b;
						return;
					}
				}
			}
			//ADebug.Print("StringBuilder new");
			sb = _sb = new StringBuilder(capacity);
		}

		/// <summary>
		/// Releases the StringBuilder to the cache.
		/// </summary>
		public void Dispose()
		{
			if(_sb.Capacity <= MAX_BUILDER_SIZE) {
				//ADebug.Print("StringBuilder released, alt=" + (t_cachedInstance != null));
				if(t_cachedInstance == null) t_cachedInstance = _sb; else t_cachedInstance2 = _sb;
			}
			_sb = null;
		}
	}
}
