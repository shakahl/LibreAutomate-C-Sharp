#define USETESTAPI

// Windows API for C#.
// Converted commonly used Windows 10 SDK header files.

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using wnd = System.IntPtr;

//add this to projects that will use these API
[module: DefaultCharSet(CharSet.Unicode)]

#if USETESTAPI

class Api2
{
	[DllImport("user32.dll", EntryPoint = "SendMessageW")]
	public static extern nint SendMessage(wnd hWnd, uint Msg, nint wParam, nint lParam);

}
#endif
