/exe
 out

str code=
 using System;
 //using System.Windows.Forms;
 class Test
 {
 #if NOARGS
       static public int Main()
       {
 return 100;
       }
 #else
       //static public int Main(string[] p)
       static int Main(string[] p)
       {
 int i; for(i=0; i<p.Length; i++) Console.WriteLine(p[i]);
 //System.IO.File.Delete("?jkjkj*hddddddddddddddddddd"); //error
 return 200;
       }
 #endif
 }


PF
rep 1
	out CsExec("")
	out CsExec("" "/5 /kk")
	 out CsExec(code)
	 out CsExec(code "/5 /kk")
	 out CsExec(code " ")
	 CsExec("macro:cs exe win")
	 CsExec("macro:cs macro")
	 CsExec("q:\app\hello.cs")
	 CsExec("$qm$\hello.cs")
	 CsExec("%app%\hello.cs")
	 CsExec("\\?\q:\app\hello.cs")
	 CsExec(":10 $qm$\hello.cs")
PN
PO
#ret
 using System; 
 //using System.Windows.Forms;
 class Test
 {
 #if NOARGS
       static public int Main()
       {
 return 100;
       }
 #else
       //static public int Main(string[] p)
       static int Main(string[] p)
       {
 int i; for(i=0; i<p.Length; i++) Console.WriteLine(p[i]);
 //System.IO.File.Delete("?jkjkj*hddddddddddddddddddd"); //error
 return 200;
       }
 #endif
 }
