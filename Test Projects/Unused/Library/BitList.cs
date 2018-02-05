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

using Au;
using static Au.NoClass;

namespace Au.Util
{
	//[DebuggerStepThrough]
	public class BitList
	{
		//List<byte> _a;
		byte[] _a;
		int _bitCount;

		public void Add(int bitCount, int bits)
		{
			if((uint)bitCount > 32) throw new ArgumentOutOfRangeException();
			int n = (_bitCount + bitCount + 7) / 8;
			int nResize = 0; if(_a == null) nResize = 32; else if(_a.Length < n) nResize = _a.Length * 2;
			if(nResize > 0) Array.Resize(ref _a, nResize);
		}
	}
}
