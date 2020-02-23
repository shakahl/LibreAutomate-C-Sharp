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
	/// Activates our manifest which tells to use comctl32.dll version 6.
	/// The manifest is embedded in this dll, resource id 2.
	/// </summary>
	internal class ActCtx_ :IDisposable
	{
		IntPtr _cookie;

		public static ActCtx_ Activate()
		{
			if(s_actCtx.Handle == default || !Api.ActivateActCtx(s_actCtx.Handle, out var cookie)) return default;
			return new ActCtx_() { _cookie = cookie };
		}

		public void Dispose()
		{
			if(_cookie != default) {
				Api.DeactivateActCtx(0, _cookie);
				_cookie = default;
			}
		}

		static ActCtx s_actCtx = new ActCtx();

		class ActCtx
		{
			public IntPtr Handle;

			public ActCtx()
			{
				Api.ACTCTX a = default;
				a.cbSize = Api.SizeOf<Api.ACTCTX>();
				a.lpSource = Assembly.GetExecutingAssembly().Location;
				a.lpResourceName = (IntPtr)2;
				a.dwFlags = Api.ACTCTX_FLAG_RESOURCE_NAME_VALID;

				var h = Api.CreateActCtx(a);
				if(h != (IntPtr)(-1)) Handle = h;
			}

			~ActCtx()
			{
				if(Handle != default) {
					Api.ReleaseActCtx(Handle);
					Handle = default;
				}
			}
		}
	}

}
