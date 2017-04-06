 #opt dispatch 1

str code=
 using System;
 public class Class1
 {
 decimal a, b, c, d, e;
 public static Class1 RetObject() { return new Class1(); }
 //public static void ArgObject(Class1 c) {  }
 public void ArgObject(Class1 c) {  }
 }
CsScript x.AddCode(code)
IDispatch D=x.Call("RetObject")

rep 100000
 rep 5
	 IDispatch d=x.Call("RetObject")
	 D.ArgObject(d)
	D.ArgObject(x.Call("RetObject"))
	
	 Acc ac.FromWindow(_hwndqm)
	 IDispatch d=ac.a
	 outref ac.a
	 x.ArgObject(ac.a) ;;OK
	  x.ArgObject(d)
	 x.ArgObject(RetInterface)
	 outref ac.a
	
	 IDispatch d=x.RetObject
	 VARIANT d=x.RetObject
	 out d
	 outref d
	 outref d.pdispVal
	 x.ArgObject(d) ;;leak. Same in VBScript.
	 x.ArgVariant(d)
	 x.Abc(d)
	 outref d
	
	 IXml x=CreateXml
	  outref x
	  IXml x=RetXmlIUnknown
	 ArgXml x
	 IUnknown u=x; ArgXml u;; u=0
	  ArgXml RetXmlIUnknown
	  outref x
	
	 IShellLink x._create(CLSID_ShellLink)
	  outref x
	 IPersistFile p=+x; p=0
	 ArgIPersistFile +x
	  outref x
	  Shell32.IS.ShellLinkObject x._create
