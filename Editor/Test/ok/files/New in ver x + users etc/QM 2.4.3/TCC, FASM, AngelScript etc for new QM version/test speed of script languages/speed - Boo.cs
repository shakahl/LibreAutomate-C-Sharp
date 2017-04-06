import System
import System.Text
import System.Diagnostics
import System.Runtime.CompilerServices


class Perf:
	static _a=array(long, 11);
	static _counter as uint=0;
	static _freq as double=1000000.0/Stopwatch.Frequency;
	
	static public def First():
		_counter=0; _a[0]=Stopwatch.GetTimestamp()
	static public def Next():
		if _counter<10: _a[++_counter]=Stopwatch.GetTimestamp()
	static public def Out():
		n=_counter;

		//calc time spans; never mind if don't need, it's fast
		for i in range(0, _counter): _a[i]=_a[i+1]-_a[i]

		s=StringBuilder("speed:")
		for i in range(0, _counter):
			s.Append("  ")
			s.Append((_freq*_a[i]-0.5) cast int)

		Console.WriteLine(s);
	
	static public def NextOut():
		Next(); Out();



public class C:

	public static def Main():
		s = string(char('a'), 10000)
		a as (int) = array(int, 10000)
		i as int
		j = 1
		b = false
		i = 0
		goto converterGeneratedName3
		while true:
			i += 1
			:converterGeneratedName3
			break  unless (i <= 9999)
			j *= 2
		Perf.First()
		i = 0
		goto converterGeneratedName4
		while true:
			i += 1
			:converterGeneratedName4
			break  unless (i <= 9999)
			j *= 2
		Perf.Next()
		i = 0
		goto converterGeneratedName5
		while true:
			i += 1
			:converterGeneratedName5
			break  unless (i <= 9999)
			if j != 0:
				b = ((j != 0) and (((j * 2) + 1) > 1))
		Perf.Next()
		i = 0
		goto converterGeneratedName6
		while true:
			i += 1
			:converterGeneratedName6
			break  unless (i <= 9999)
			j += s[i] cast int
		Perf.Next()
		i = 0
		goto converterGeneratedName7
		while true:
			i += 1
			:converterGeneratedName7
			break  unless (i <= 9999)
			j += a[i]
		Perf.Next()
		i = 0
		goto converterGeneratedName8
		while true:
			i += 1
			:converterGeneratedName8
			break  unless (i <= 9999)
			j = Test()
			j = Test1(i)
			j = Test2(i, 0)
			j = Test3(i, j, 0)
			j = Test4(i, j, 1000000, 0)
		Perf.Next()
		Perf.Out()

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static def Test() as int:
		return 1

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static def Test1(a as int) as int:
		return (-1)

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static def Test2(a as int, b as int) as int:
		return (a + b)

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static def Test3(a as int, b as int, c as int) as int:
		return ((a + b) + c)

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static def Test4(a as int, b as int, c as int, d as int) as int:
		return (((a + b) + c) + d)
	


for v in range(0, 10):
	C.Main()
