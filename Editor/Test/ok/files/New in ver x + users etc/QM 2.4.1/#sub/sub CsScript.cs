/exe
sub.Test

#sub Test
str variable="a b c"
int R=CsExec("" F"/test ''{variable}''")
out R

 BEGIN PROJECT
 main_function  sub CsScript
 exe_file  $my qm$\sub CsScript.qmm
 flags  6
 guid  {954A333A-688A-41C1-8F41-1117F08A9D73}
 END PROJECT

#ret
//C# code
using System;
using System.Windows.Forms;
class Test
{
	static int Main(string[] p)
	{
		int i; for(i=0; i<p.Length; i++) Console.WriteLine(p[i]); //display in QM output
		MessageBox.Show("Passed " + p.Length + " arguments.", "CsExec");
		return 100;
	}
}