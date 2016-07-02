using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Wnd = System.IntPtr; //HWND (window handle)

//[DebuggerStepThrough]
public class Class1
{
	Wnd hwnd = API.FindWindow("QM_Editor", null);
}
