#define TEST_CONST

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
using System.ComponentModel; //Win32Exception

//using System.Reflection;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;

#pragma warning disable 169

namespace SdkConverter
{
	unsafe partial class Converter
	{
		struct ARR
		{
			int a0, a1, a2, a3;

			public int this[int i]
			{
				get { fixed (int* p = &a0) { return p[i]; } }
				set { fixed (int* p = &a0) { p[i] = value; } }
			}
		}

		struct ARRHOLDER
		{
			public _int_100 a;
			public struct _int_100
			{
				int a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20, a21, a22, a23, a24, a25, a26, a27, a28, a29, a30, a31, a32, a33, a34, a35, a36, a37, a38, a39, a40, a41, a42, a43, a44, a45, a46, a47, a48, a49, a50, a51, a52, a53, a54, a55, a56, a57, a58, a59, a60, a61, a62, a63, a64, a65, a66, a67, a68, a69, a70, a71, a72, a73, a74, a75, a76, a77, a78, a79, a80, a81, a82, a83, a84, a85, a86, a87, a88, a89, a90, a91, a92, a93, a94, a95, a96, a97, a98, a99;

				public int this[int i]
				{
					get { fixed (int* p = &a0) { return p[i]; } }
					set { fixed (int* p = &a0) { p[i] = value; } }
				}
			}

			[MarshalAs(UnmanagedType.ByValArray, SizeConst =100)]
            public int[] b;

			public struct STRU
			{
				public int i;
				//string s;
			}

			public _STRU_4 c;
			public struct _STRU_4
			{
				STRU a0, a1, a2, a3;

				public STRU this[int i]
				{
					get { fixed (STRU* p = &a0) { return p[i]; } }
					set { fixed (STRU* p = &a0) { p[i] = value; } }
				}
			}

			[MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
            public STRU[] d;
		}

		public static Guid IID_IPersist = new Guid("0000010c-0000-0000-C000-000000000046");

		[DllImport("dddd")]
		static extern void TestReadonlyRef([In] ref Guid g);

		void Test()
		{
			//var v = new ARRHOLDER();
			//v.c[0].i = 5;

			TestReadonlyRef(ref IID_IPersist);

			return;


			//var x = new ARR();
			//for(int i=0; i<4; i++) {
			//	x[i] = i + 100;
			//	Out(x[i]);
			//}

			int n1 = 0, n2 = 0;

			var x = new ARRHOLDER();

			//x.b = new int[100];
			//for(int i = 0; i < 100; i++) {
			//	x.a[i] = i + 100;
			//	Out(x.a[i]);
			//}
			//Out("DONE");

			var a1 = new Action(() =>
			{
				for(int i = 0; i < 100; i++) {
					x.a[i] = i + 100;
					n1 += x.a[i];
				}
			});

			var a2 = new Action(() =>
			{
			x.b = new int[100];
				for(int i = 0; i < 100; i++) {
					x.b[i] = i + 100;
					n2 += x.b[i];
				}
			});

			Perf.ExecuteMulti(5, 50, a1, a2);

			OutList(n1, n2);
		}
	}
}

//public unsafe class Pointers_
//{
//struct HASARRAY
//{
//	//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
//	fixed int a[5];
//}

//struct USEHASARRAY
//{
//	HASARRAY* p;
//}

//}


#if TEST_CONST

class CONSTANTS
{

}

#endif
