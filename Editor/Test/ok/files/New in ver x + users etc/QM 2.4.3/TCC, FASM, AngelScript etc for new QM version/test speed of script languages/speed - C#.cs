CsScript x.Init
 x.SetOptions("debugConfig=true")
x.AddCode("")
WakeCPU
x.Call("C.Main")


#ret
//C# code
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

	class Perf
	{
		static long[] _a= new long[11];
		static uint _counter=0;
		static double _freq=1000000.0/Stopwatch.Frequency;

		//static public Perf()
		//{
		//}

		static public void First() { _counter=0; _a[0]=Stopwatch.GetTimestamp(); }
		static public void Next() { if(_counter<10) _a[++_counter]=Stopwatch.GetTimestamp(); }
		static public void Out()
		{
			uint i, n=_counter;

			//calc time spans; never mind if don't need, it's fast
			for(i=0; i<n; i++) _a[i]=_a[i+1]-_a[i];

			StringBuilder s=new StringBuilder("speed:");
			for(i=0; i<n; i++)
			{
				s.Append("  ");
				s.Append((int)(_freq*_a[i]-0.5));
			}

			Console.WriteLine(s);
		}
		static public void NextOut() { Next(); Out(); }
	}


public class C
{
public static void Main()
{
string s=new string('a', 10000);
int[] a=new int[10000];

//Test(); Test1(1); Test2(1, 1); Test3(1, 1, 1); Test4(1, 1, 1, 1);

int i, j=1; bool b=false;
for(i=0; i<10000; i++) j*=2;

Perf.First();
//loop
for(i=0; i<10000; i++) j*=2;
Perf.Next();
//if, expression
for(i=0; i<10000; i++) { if(j!=0) b=(j!=0) && (j*2+1 > 1); }
Perf.Next();
//string iteration
for(i=0; i<10000; i++) j+=s[i];
Perf.Next();
//array iteration
for(i=0; i<10000; i++) j+=a[i];
Perf.Next();
//function call
for(i=0; i<10000; i++)
{
	j=Test();
	j=Test1(i);
	j=Test2(i, 0);
	j=Test3(i, j, 0);
	j=Test4(i, j, 1000000, 0);
}
Perf.Next();
Perf.Out();
}

[MethodImpl(MethodImplOptions.NoInlining)] static int Test() { return 1; }
[MethodImpl(MethodImplOptions.NoInlining)] static int Test1(int a) { return -1; }
[MethodImpl(MethodImplOptions.NoInlining)] static int Test2(int a, int b) { return a+b; }
[MethodImpl(MethodImplOptions.NoInlining)] static int Test3(int a, int b, int c) { return a+b+c; }
[MethodImpl(MethodImplOptions.NoInlining)] static int Test4(int a, int b, int c, int d) { return a+b+c+d; }

}
