 //#define TRACE
 using System;
 //using System.Windows.Forms;

 public class Test  
 {
       static public int Add(int a, int b)
       {
          //return a+b //
          //string s="ąčę"; return a+b //.........
	//return a+b //..............
	return a/b;
       }
       
 
       //static public string MsgBox(string s)
       static public string MsgBox(int a, int b)
       {
          //MessageBox.Show(s);
          return "returned: " + (a+b);
       }

       static public int Find(int x)
       {
       int i;
       for(i=0; i<1000; i++) if(i==x) return i;
       return -5;
       }

       static public void Output()
       {
          Console.WriteLine("console");
          System.Diagnostics.Trace.WriteLine("trace");
       }

       static public void AnyType(object v)
       {
          Console.WriteLine(v);
       }

       static public int Over(int x) { return x; }
       static public int Over(string x) { return x.Length; }

 }
