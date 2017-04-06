/exe
 out

str code=
 using System;
 public class Test
 {
     public int Add(int a, int b) { return a+b; }
     public int Prop { set; get; }
     public int Over(int x) { return x; }
     public int Over(string x) { return x.Length; }
     public bool Boool(bool x) { return x; }
     public DateTime DT(DateTime x) { Console.Write(x); return x; }
     public enum Enu { one, two }
     public Enu EN(Enu x) { return x; }
 }

PF
CsScript x.AddCode(code)
 PN
 ret
IDispatch o=x.CreateObject("Test")
PN
int R
rep 5
	R=o.Add(2 3)
	 R=o.Over(5)
	 R=o.Over_2("test")
	 R=o.Boool(1)
	 DateTime d.FromComputerTime; DateTime dd.t=o.DT(d.ToDATE); out dd.ToStr(4)
	 DateTime d.FromComputerTime; DateTime dd.t=o.DT(d); out dd.ToStr(4) ;;unsupported argument type. Should unalias. But does not convert to DATE anyway.
	 R=o.EN(1)
	PN
o.Prop=100; int P=o.Prop
PN
PO
out R
out P

 BEGIN PROJECT
 main_function  test CsScript interface IDispatch
 exe_file  $my qm$\test CsScript interface IDispatch.qmm
 flags  6
 guid  {AA07CBB3-CC8E-4A1E-8A3B-0692D834F4C6}
 END PROJECT
