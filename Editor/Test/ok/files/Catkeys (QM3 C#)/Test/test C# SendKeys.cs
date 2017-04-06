int R=CsExec("")
out R

#ret
//C# code
using System;
using System.Diagnostics;
using System.Windows.Forms;

class Test
{
static void Main()
{
//SendKeys.SendWait("{LEFT}");
	//MessageBox.Show("kk");
}
}

static public class Perf
{
	static long[] _a= new long[11];
	static uint _counter=0;
	static double _freq=1000000.0/Stopwatch.Frequency;
}
