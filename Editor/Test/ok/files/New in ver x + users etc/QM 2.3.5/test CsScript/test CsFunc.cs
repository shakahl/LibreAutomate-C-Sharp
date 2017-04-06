/exe
 out

str classlessFunction="int Add(int a, int b) { return a+b; }"
 str classlessFunction="//class ggg[]static public int Add(int a, int b) { return a+b; }"
 str classlessFunction="using System;[]public static int Add(int a, int b) { return a+b; }"
 str classlessFunction="using System;[]int Add(int a, int b) { return a+b; }"
int R=CsFunc(classlessFunction 2 3)
out R

str normalCode=
 using System;
 public class Test
 {
   static public int Add(int a, int b)
   {
     _Out(a + " + " + b);
     return a+b;
   }
   static void _Out(object x) { Console.WriteLine(x.ToString()); }
 }
out CsFunc(normalCode 20 30)
