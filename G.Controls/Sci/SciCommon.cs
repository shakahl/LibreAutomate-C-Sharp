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
using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

using ScintillaNET;

namespace G.Controls
{
	//[DebuggerStepThrough]
	internal static class SciCommon
	{
		//public const int SCI_UPDATESCROLLBARS= 9501; //not impl
		public const int SCI_MARGINSTYLENEXT = 9502;
		//public unsafe delegate void NotifyCallback(void* cbParam, ref SCNotification n);
		//public const int SCI_SETNOTIFYCALLBACK = 9503;
		public unsafe delegate int AnnotationDrawCallback(void* cbParam, ref AnnotationDrawCallbackData c);
		public const int SCI_SETANNOTATIONDRAWCALLBACK = 9504;
		public const int SCI_ISXINMARGIN = 9506;
		//these not impl
		//public const int SC_DOCUMENT_USERDATA_OFFSET= 12;
		//public const int SC_DOCUMENT_USERDATA_SIZE= 4;

#pragma warning disable 649
		public unsafe struct AnnotationDrawCallbackData
		{
			public int step;
			public IntPtr hdc;
			public RECT rect;
			public sbyte* text;
			public int textLen, line, annotLine;
		};
#pragma warning restore 649

	}
}
