/exe
out
str code=
 using System;
 public class Test
 {
 int m;
 public int Add(int a, int b) { return a+b; }
 public int Add(int a) { return a; }
 
 public bool f_bool(bool b) { Console.Write(b); return b; }
 public byte f_byte(byte b) { Console.Write(b); return b; }
 public char f_char(char b) { Console.Write("char="+b); return b; }
 public decimal f_decimal(decimal b) { Console.Write(b); return b; }
 public double f_double(double b) { Console.Write(b); return b; }
 public enum Enu { one, two }
 public int f_enum(Enu e) { return (int)e; }
 public Enu f_enum2(Enu e) { return e; } //slower
 public float f_float(float b) { Console.Write(b); return b; }
 public long f_long(long b) { Console.Write(b); return b; }
 public sbyte f_sbyte(sbyte b) { Console.Write(b); return b; }
 public short f_short(short b) { Console.Write(b); return b; }
 public uint f_uint(uint b) { Console.Write(b); return b; }
 public ulong f_ulong(ulong b) { Console.Write(b); return b; }
 public ushort f_ushort(ushort b) { Console.Write(b); return b; }
 
 public string f_string(string s) { Console.Write(s); return s; }
 public object f_object(object s) { Console.Write(s); return s; }
 public DateTime f_date(DateTime s) { Console.Write(s); return s; }
 
 public Test f_get_class() { return new Test(); }
 public void f_class(Test k) { Console.Write(k.m); }
 public void Meth(int i) { m=i; }
 
 public int f_array_int(int[] a) { Console.Write(a[4]); return a.Length; }
 public int f_array_str(string[] a) { Console.Write(a[4]); return a.Length; }
 
 public struct Stru { public int x; public int y; }
 public void f_struct(Stru b) { Console.Write(b.y); }
 public Stru f_struct2() { Stru k; k.x=1; k.y=2; return k; }
 }

PF
CsScript x.AddCode(code)
IDispatch d=x.CreateObject("Test")
PN
 int R

rep 1
	int R=d.Add(10 5)
	 int R=d.Add_2(3)
	
	 int R=d.f_bool(-1)
	 byte R=d.f_bool("true")
	 int R=d.f_byte(200)
	 int R=d.f_byte(-1) ;;overflow
	 int R=d.f_char('A')
	 DECIMAL R=d.f_decimal(10.5)
	 DECIMAL R=1000.55; R=d.f_decimal(R)
	 double R=d.f_double(10.5)
	 double R=d.f_double("10.5E2")
	 FLOAT f=10.7; double R=d.f_double(f)
	 int R=d.f_enum(10)
	 int R=d.f_enum2(10)
	 FLOAT R=d.f_float(10.7)
	 double R=d.f_float(10.7)
	 long R=d.f_long(1000000000000L)
	 VARIANT R=d.f_long(1000000000000L); out R.vt
	 int R=d.f_sbyte(-1)
	 int R=d.f_short(-1)
	 word w=0xffff; int R=d.f_short(w)
	 int R=d.f_uint(0xffffffff)
	 int R=d.f_uint(0xffffffffL)
	 long R=d.f_ulong(1000000000000)
	 VARIANT R=d.f_ulong(1000000000000); out R.vt
	 int R=d.f_ushort(1)
	
	 str R=d.f_string("test")
	 str R=d.f_string(10)
	 VARIANT R=d.f_object(10)
	 VARIANT R=d.f_object("hhh")
	 DateTime D.FromComputerTime; DateTime R.t=d.f_date(D.ToDATE); out R.ToStr(4)
	 DateTime R.t=d.f_date("2000.05.01"); out R.ToStr(4)
	 DateTime R.FromDATE(d.f_date("2000.05.01")); out R.ToStr(4)
	 DATE D.getclock; DATE R=d.f_date(D)
	
	 IDispatch R=d.f_get_class()
	 R.Meth(100)
	 d.f_class(R)
	
	 ARRAY(int) a.create(5); a[4]=40; int R=d.f_array_int(a)
	 ARRAY(BSTR) a.create(5); a[4]="test"; int R=d.f_array_str(a)
	 ARRAY(str) a.create(5); a[4]="test"; int R=d.f_array_str(a)
	
	 int R; POINT p; d.f_struct(p) ;;error
	 int R; POINT p=d.f_struct2() ;;error
	PN
 PO
out R

 BEGIN PROJECT
 main_function  test CsScript Call cast static
 exe_file  $my qm$\test CsScript Call cast.qmm
 flags  6
 guid  {7E61E6E6-3D80-47AB-90E2-3A0F322337E6}
 END PROJECT
