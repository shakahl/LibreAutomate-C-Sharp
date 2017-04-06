 #opt dispatch 1
typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
Project1.Class1 x._create
 IDispatch x._create("Lietuviškas.Class1")

 rep 20000
 rep 5
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
	
	
