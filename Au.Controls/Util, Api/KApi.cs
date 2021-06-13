using Au.Types;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Au.Controls
{
	[DebuggerStepThrough]
	static unsafe class KApi
	{

		[DllImport("user32.dll")]
		internal static extern bool DrawFrameControl(IntPtr hdc, in RECT r, int type, int state);



	}

}
