using System;
using System.Collections.Generic;
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

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

[module: DefaultCharSet(CharSet.Unicode)]

namespace Editor
{
	internal static class _Api
	{
		public struct NMHDR
		{
			public Wnd hwndFrom;
			public LPARAM idFrom;
			public uint code;
		}

		public struct TVITEMEX
		{
			public uint mask;
			public IntPtr hItem;
			public uint state;
			public uint stateMask;
			public IntPtr pszText;
			public int cchTextMax;
			public int iImage;
			public int iSelectedImage;
			public int cChildren;
			public LPARAM lParam;
			public int iIntegral;
			public uint uStateEx;
			public Wnd hwnd;
			public int iExpandedImage;
			public int iReserved;
		}

		public struct TVINSERTSTRUCT
		{
			public IntPtr hParent;
			public IntPtr hInsertAfter;
			public TVITEMEX item;
		}

		public const uint TVM_SETIMAGELIST = 0x1109;
		public const int TVSIL_NORMAL = 0;
		public const uint TVM_SELECTITEM = 0x110B;
		public const uint TVGN_CARET = 0x9;
		public const int LPSTR_TEXTCALLBACK = -1;
		public const int TVI_LAST = unchecked((int)0xFFFF0002); //note: not uint, because then overflow exception (or very slow) when assigning to IntPtr
		public const uint TVM_INSERTITEM = 0x1132;
		public const uint TVIF_TEXT = 0x1;
		public const uint TVIF_PARAM = 0x4;
		public const uint TVIF_IMAGE = 0x2;
		public const uint TVIF_SELECTEDIMAGE = 0x20;

		public const uint TVS_INFOTIP = 0x800;
		public const uint TVS_FULLROWSELECT = 0x1000;
		public const uint TVS_SHOWSELALWAYS = 0x20;
		public const uint TVS_HASBUTTONS = 0x1;
		public const uint TVS_HASLINES = 0x2;
		public const uint TVS_LINESATROOT = 0x4;
		public const uint TVS_EDITLABELS = 0x8;
		public const uint TVS_SINGLEEXPAND = 0x400;
		public const uint TVS_TRACKSELECT = 0x200;

		//public const uint TVM_SETEXTENDEDSTYLE = 0x112C;
		//public const uint TVS_EX_FADEINOUTEXPANDOS = 0x40;

		//[DllImport("ole32.dll")]
		//public static extern void OleUninitialize();
		//[DllImport("ole32.dll", PreserveSig = true)]
		//public static extern int OleInitialize(IntPtr pvReserved);

		public const uint EM_SETCUEBANNER = 0x1501;





		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(Wnd hWnd);
		[DllImport("user32.dll")]
		public static extern int ReleaseDC(Wnd hWnd, IntPtr hDC);

		//[DllImport("user32.dll")]
		//public static extern Wnd GetCapture();
		//[DllImport("user32.dll")]
		//public static extern Wnd SetCapture(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern bool DragDetect(Wnd hwnd, POINT pt);




	}
}
