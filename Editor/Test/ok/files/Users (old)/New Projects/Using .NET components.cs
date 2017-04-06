 From http://www.15seconds.com/Issue/010214.htm
 Could not make this to work. _create says "The system cannot find the file specified".

 ChDir "$windows$\microsoft.net\framework\v2.0.50727" ;;enable this if not set in PATH env var
str f.expandpath("$desktop$\netcomp") cl

sel list("create cs[]compile cs[]regasm[]run")
	case 1
	mkdir f
	str s=
 using System;
 namespace ManagedServer
 {
    public class CManagedServer
    {
      public CManagedServer()
      {}
      public string SayHello(string r_strName) 
      {
	 string str ;
	 str = "Hello " + r_strName ;
	 return str ;     
      }
    }
 }
	s.setfile(_s.from(f "\ManagedServer.cs"))
	
	case 2
	cl.format("csc /out:%s\ManagedServer.dll /target:library %s\ManagedServer.cs" f f)
	 out cl
	RunConsole2 cl
	
	case 3
	cl.format("regasm %s\ManagedServer.dll /tlb" f)
	 out cl
	RunConsole2 cl
	
	case 4
	  enable this when tlb created
	typelib ManagedServer {AE901302-A331-382E-812D-B108E4F90BE6} 1.0
	ManagedServer.CManagedServer ms._create ;;0x80070002, The system cannot find the file specified.
	
