/exe
 out

str code=
 using System;
 using System.Runtime.InteropServices;
 namespace NS{
 struct POINT { public int x, y; }
 [Guid("C7D9B459-0108-4F4A-9483-A7F60F18AF02")]
 public interface ITest
 {
     int Add(int a, int b);
     int Prop { set; get; }
     int Over(int x);
     int Over(string x);
     object Arr();
 }
 public class Test : ITest
 {
     //public Test() { Console.Write("ctor"); }
     public int Add(int a, int b)
     {
     return a+b;
     }
     public int Prop { set; get; }
     public int Over(int x) { return x; }
     public int Over(string x) { return x.Length; }
     public object Arr() { return new string[] {"one", "two"}; }
     //public object Arr() { return new POINT[1]; }
 }}

PF
CsScript x.AddCode(code)
PN
interface# ITest :IDispatch
	#Add(a b)
	[p]Prop(x)
	[g]#Prop
	#Over(x)
	#Over_2(BSTR's)
	`Arr()
	{C7D9B459-0108-4F4A-9483-A7F60F18AF02}
ITest o=x.CreateObject("NS.Test")
PN
rep 5
	int R=o.Add(2 3)
	 int R=o.Over(5)
	 int R=o.Over_2("test") ;;support overloads, but need to change function name in QM interface definition.
	PN
o.Prop=100; int P=o.Prop
PN
PO
out R
out P

VARIANT v=o.Arr
out F"0x{v.vt}   0x{v.parray.psa.fFeatures} {v.parray.psa.cbElements}"

 notes:
 Calling through VTBL is not so easy as through IDispatch, but >10 times faster.
 In the script need to define interface with GUID. Maybe C# compiler would generate default interface, but then we would not know its GUID.
 We need GUID because o=d.CreateObject calls QI. Cannot assign the returned IDispatch without QI; then QM crashes.

 BEGIN PROJECT
 main_function  test CsScript interface vtbl
 exe_file  $my qm$\test CsScript interface vtbl.qmm
 flags  6
 guid  {B39D929D-D764-49A0-8DB7-3BA64BED98E1}
 END PROJECT
