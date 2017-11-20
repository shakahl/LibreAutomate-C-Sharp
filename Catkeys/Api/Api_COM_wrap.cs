using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys.Types
{
	internal class LibDelegateTypes
	{
		internal delegate int IntPtr_out_BSTR(IntPtr obj, out BSTR value);
		internal delegate int IntPtr_out_string(IntPtr obj, [MarshalAs(UnmanagedType.BStr)] out string value);
		internal delegate int IntPtr_(IntPtr obj);
		internal delegate int IntPtr_int(IntPtr obj, int i);

	}

	internal static unsafe partial class Api
	{
		internal unsafe struct IEnumVARIANT :IDisposable
		{
			IntPtr _iptr;

			public static bool From(IntPtr iUnknown, out IEnumVARIANT x)
			{
				return Util.Marshal_.QueryInterface(iUnknown, out x, ref IID_IEnumVARIANT);
			}

			internal static Guid IID_IEnumVARIANT = new Guid(0x00020404, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

			public void Dispose()
			{
				if(_iptr != default) {
					Marshal.Release(_iptr);
					_iptr = default;
				}
			}

			public bool Is0 => _iptr == default;

			public static implicit operator IntPtr(IEnumVARIANT x) => x._iptr;

			public int Next(int celt, out VARIANT rgVar, out int pCeltFetched)
			{
				return _F.Next(_iptr, celt, out rgVar, out pCeltFetched);
			}

			public int Skip(int celt)
			{
				return _F.Skip(_iptr, celt);
			}

			public int Reset()
			{
				return _F.Reset(_iptr);
			}

			static ConcurrentDictionary<LPARAM, _Vtbl> s_vtbls = new ConcurrentDictionary<LPARAM, _Vtbl>();

			_Vtbl _F
			{
				get
				{
					if(_iptr == default) throw new ObjectDisposedException(nameof(IEnumVARIANT));
					return s_vtbls.GetOrAdd(*(IntPtr*)_iptr, vtbl => new _Vtbl(vtbl));
				}
			}

			class _Vtbl
			{
				public _Vtbl(long vtbl)
				{
#if DEBUG
					int n = s_vtbls.Count; if(n > 0) Debug_.Print("many VTBLs: " + (n + 1));
#endif
					var a = (IntPtr*)vtbl;
					Util.Marshal_.GetDelegate(a[3], out Next);
					Util.Marshal_.GetDelegate(a[4], out Skip);
					Util.Marshal_.GetDelegate(a[5], out Reset);
				}

				internal readonly NextT Next;
				internal readonly LibDelegateTypes.IntPtr_int Skip;
				internal readonly LibDelegateTypes.IntPtr_ Reset;

				internal delegate int NextT(IntPtr obj, int celt, out VARIANT rgVar, out int pCeltFetched);

				//[ComImport, Guid("00020404-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
				//internal interface IEnumVARIANT
				//{
				//	[PreserveSig] int Next(uint celt, ref object rgVar, out uint pCeltFetched);
				//	[PreserveSig] int Skip(uint celt);
				//	[PreserveSig] int Reset();
				//	[PreserveSig] int Clone(out IEnumVARIANT ppEnum);
				//}
			}
		}
	}
}
