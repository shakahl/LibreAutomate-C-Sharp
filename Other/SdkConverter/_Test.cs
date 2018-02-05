//#define TEST_TYPEFUNC
#define TEST_CONST

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
using System.ComponentModel; //Win32Exception

//using System.Reflection;
//using System.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

#pragma warning disable 169

namespace SdkConverter
{
	unsafe partial class Converter
	{
#if TEST_TYPEFUNC

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

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
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

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public STRU[] d;
		}

		public static Guid IID_IPersist = new Guid("0000010c-0000-0000-C000-000000000046");


		[DllImport("kernel32.dll", EntryPoint = "GetCommandLineW")]
		//public static extern string GetCommandLine(); //exception
		//public static extern StringBuilder GetCommandLine(); //exception
		//public static extern IntPtr GetCommandLine(); //OK
		public static unsafe extern char* GetCommandLine(); //OK

		[DllImport("user32.dll", EntryPoint = "CharUpperW", CharSet = CharSet.Unicode)]
		public static extern IntPtr CharUpper([In] StringBuilder lpsz);


		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", EntryPoint = "TestUnmanaged", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestUnmanaged1(ref SRKH k, ref Guid g);
		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", EntryPoint = "TestUnmanaged", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestUnmanaged2([In] ref SRKH k, [In] ref Guid g);
		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", EntryPoint = "TestUnmanaged", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestUnmanaged3(out SRKH k, out Guid g);
		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", EntryPoint = "TestUnmanaged", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestUnmanaged4([In, Out] ref SRKH k, [In, Out] ref Guid g);

		struct SRKH
		{
			long q, w, e, r, t, y, u, i, o, p, a, s, d, f, g, h, j, k;
		}

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestStructBlit(ref POINT p);

		struct STSTR
		{
			public int k;
			public string s;
		};

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestStructString(ref STSTR p); //if struct, default is [In,Out], even if has non-blittable members. Can apply [In] to prevent copying non-blittable types back.

		[StructLayout(LayoutKind.Sequential)]
		class STSTR2
		{
			public int k;
			public string s;
		};

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", EntryPoint = "TestStructString", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestStructString2(STSTR2 p); //if class, default is [In], except for StringBuilder

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestArray(int[] a);

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void TestArrayStr([In, Out]string[] a);

		[ComImport, Guid("3AB5235E-2768-47A2-909A-B5852A9D1868"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		interface ITest
		{
			[PreserveSig]
			int Test1(int i);
			[PreserveSig]
			//int Test2(int* p);
			int Test2([MarshalAs(UnmanagedType.LPArray)] int[] p);
		};

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern ITest CreateTestInterface();

		[DllImport(@"Q:\app\Au\Test Projects\UnmanagedDll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern ITest TestSimple();

#endif

		void Test()
		{
			//TestSimple();

			//var x = CreateTestInterface();
			//x.Test1(3);
			//int[] a = { 1, 2 };
			////fixed(int* p = a)
			////{
			////	x.Test2(p);
			////}
			//x.Test2(a);

			//string[] a = { "one", "two" };
			//TestArrayStr(a);
			//Print(a[0]);
			//int[] a = { 1, 2 };
			//TestArray(a);
			//Print(a[0]);

			//string s = "instr";
			//var p = new STSTR();
			////var p = new STSTR2();
			//p.k = 1;
			//p.s = s;
			//TestStructString(ref p);
			////TestStructString2(p);
			//Print("returned");
			//OutList(p.k, p.s);
			//Print(p.s == s);

			//var p = new POINT();
			//p.x = 2;
			//TestStructBlit(ref p);
			//Print(p.y);


			//var v = new SRKH();
			//var g = new Guid();

			//var a1 = new Action(() => { TestUnmanaged1(ref v, ref g); });
			//         var a2 = new Action(() => { TestUnmanaged2(ref v, ref g); });
			//var a3 = new Action(() => { TestUnmanaged3(out v, out g); });
			//var a4 = new Action(() => { TestUnmanaged4(ref v, ref g); });

			//Perf.ExecuteMulti(5, 10000, a1, a2, a3, a4);

			//var sb = new StringBuilder("test");
			//CharUpper(sb);
			//Print(sb);

			//int k = 1;
			//int R=TestUnmanaged(ref k);
			//OutList(R, k);

			//bool k = false;
			//bool R=TestUnmanaged(ref k);
			//OutList(R, k);


			//Print(1);
			//string s;
			//s = new string(GetCommandLine());
			//Print(s);
			//s = new string(GetCommandLine());
			//Print(s);
			//s = new string(GetCommandLine());
			//Print(s);
			//Print(2);


			//var v = new ARRHOLDER();
			//v.c[0].i = 5;

			//return;


			////var x = new ARR();
			////for(int i=0; i<4; i++) {
			////	x[i] = i + 100;
			////	Print(x[i]);
			////}

			//int n1 = 0, n2 = 0;

			//var x = new ARRHOLDER();

			////x.b = new int[100];
			////for(int i = 0; i < 100; i++) {
			////	x.a[i] = i + 100;
			////	Print(x.a[i]);
			////}
			////Print("DONE");

			//var a1 = new Action(() =>
			//{
			//	for(int i = 0; i < 100; i++) {
			//		x.a[i] = i + 100;
			//		n1 += x.a[i];
			//	}
			//});

			//var a2 = new Action(() =>
			//{
			//x.b = new int[100];
			//	for(int i = 0; i < 100; i++) {
			//		x.b[i] = i + 100;
			//		n2 += x.b[i];
			//	}
			//});

			//Perf.ExecuteMulti(5, 50, a1, a2);

			//OutList(n1, n2);
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
