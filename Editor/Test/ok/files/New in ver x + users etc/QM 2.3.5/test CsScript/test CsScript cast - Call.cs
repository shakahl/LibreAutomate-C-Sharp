/exe
out
str code=
 using System;
 public class Test
 {
 int m;
 public static int Add(int a, int b) { return a+b; }
 public static int Add(int a) { return a; }
 
 public static bool f_bool(bool b) { Console.Write(b); return b; }
 public static byte f_byte(byte b) { Console.Write(b); return b; }
 public static char f_char(char b) { Console.Write("char="+b); return b; }
 public static decimal f_decimal(decimal b) { Console.Write(b); return b; }
 public static double f_double(double b) { Console.Write(b); return b; }
 public enum Enu { one, two }
 public static int f_enum(Enu e) { return (int)e; }
 public static Enu f_enum2(Enu e) { return e; } //slower
 public static float f_float(float b) { Console.Write(b); return b; }
 public static long f_long(long b) { Console.Write(b); return b; }
 public static sbyte f_sbyte(sbyte b) { Console.Write(b); return b; }
 public static short f_short(short b) { Console.Write(b); return b; }
 public static uint f_uint(uint b) { Console.Write(b); return b; }
 public static ulong f_ulong(ulong b) { Console.Write(b); return b; }
 public static ushort f_ushort(ushort b) { Console.Write(b); return b; }
 
 public static string f_string(string s) { Console.Write(s); return s; }
 public static object f_object(object s) { Console.Write(s); return s; }
 public static DateTime f_date(DateTime s) { Console.Write(s); return s; }
 
 public static Test f_get_class() { return new Test(); }
 public static void f_class(Test k) { Console.Write(k.m); }
 public void Meth(int i) { m=i; }
 
 public static int f_array_int(int[] a) { Console.Write(a[4]); return a.Length; }
 public static int f_array_str(string[] a) { Console.Write(a[4]); return a.Length; }
 
 public struct Stru { public int x; public int y; }
 public static void f_struct(Stru b) { Console.Write(b.y); }
 public static Stru f_struct2() { Stru k; k.x=1; k.y=2; return k; }
 }

PF
CsScript x.AddCode(code)
PN
 int R

rep 1
	int R=x.Call("Add" 10 5)
	 int R=x.Call("Add" 3)
	
	 int R=x.Call("f_bool" -1)
	 byte R=x.Call("f_bool" "true")
	 int R=x.Call("f_byte" 200)
	 int R=x.Call("f_char" 'A')
	 DECIMAL R=x.Call("f_decimal" 10.5)
	 DECIMAL R=1000.55; R=x.Call("f_decimal" R)
	 double R=x.Call("f_double" 10.5)
	 double R=x.Call("f_double" "10.5E2")
	 FLOAT f=10.7; double R=x.Call("f_double" f)
	 int R=x.Call("f_enum" 10)
	 int R=x.Call("f_enum2" 10)
	 FLOAT R=x.Call("f_float" 10.7)
	 double R=x.Call("f_float" 10.7)
	 long R=x.Call("f_long" 1000000000000L)
	 VARIANT R=x.Call("f_long" 1000000000000L); out R.vt
	 int R=x.Call("f_sbyte" -1)
	 int R=x.Call("f_short" -1)
	 word w=0xffff; int R=x.Call("f_short" w)
	 int R=x.Call("f_uint" 0xffffffff)
	 int R=x.Call("f_uint" 0xffffffffL)
	 long R=x.Call("f_ulong" 1000000000000)
	 VARIANT R=x.Call("f_ulong" 1000000000000); out R.vt
	 int R=x.Call("f_ushort" 1)
	
	 str R=x.Call("f_string" "test")
	 str R=x.Call("f_string" 10)
	 VARIANT R=x.Call("f_object" 10)
	 VARIANT R=x.Call("f_object" "hhh")
	 DateTime D.FromComputerTime; DateTime R.t=x.Call("f_date" D.ToDATE); out R.ToStr(4)
	 DateTime R.t=x.Call("f_date" "2000.05.01"); out R.ToStr(4)
	 DATE D.getclock; DATE R=x.Call("f_date" D)
	
	 IDispatch R=x.Call("f_get_class")
	 R.Meth(100)
	 x.Call("f_class" R)
	
	 ARRAY(int) a.create(5); a[4]=40; int R=x.Call("f_array_int" a)
	 ARRAY(BSTR) a.create(5); a[4]="test"; int R=x.Call("f_array_str" a)
	 ARRAY(str) a.create(5); a[4]="test"; int R=x.Call("f_array_str" a)
	
	 int R; POINT p; x.Call("f_struct" p) ;;error
	 int R; POINT p=x.Call("f_struct2") ;;error
	PN
 PO
out R

 BEGIN PROJECT
 main_function  test CsScript Call cast static
 exe_file  $my qm$\test CsScript Call cast.qmm
 flags  6
 guid  {7E61E6E6-3D80-47AB-90E2-3A0F322337E6}
 END PROJECT
