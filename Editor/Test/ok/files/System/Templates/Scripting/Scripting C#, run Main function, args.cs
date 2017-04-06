str variable="test variable"
int R=CsExec("" F"/test ''{variable}''")
out R


#ret
//C# code
using System;
using System.Windows.Forms;

class Test
{
static int Main(string[] p)
{
	int i; for(i=0; i<p.Length; i++) Console.Write(p[i]); //display in QM output
	MessageBox.Show("Passed " + p.Length + " arguments.", "CsExec");
	return 5;
}
}
