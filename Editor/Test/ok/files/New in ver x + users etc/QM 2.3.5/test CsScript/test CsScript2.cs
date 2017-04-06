/exe
out

 str code.getfile("$qm$\hello.cs")
str code.getmacro("cs1")
str sf.expandpath("$qm$\hello.cs")

PF
CsScript x.Init
PN

 rep(1000) _i=x.x.Call0("func")
 rep(1000) _i=x.x.Call4("func" 1 1 1 1)
 rep(1000) _i=x.x.Call4("func")
 rep(1000) _i=x.x.Call16("func")
 rep(1000) _i=x.x.Call16("func" 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1)
 rep(1000) Function229(x.x "func")
 rep(1000) x.Call("func")
 rep(1000) x.Call("func" 1 1 1 1 1 1 1 1 1 1)
 rep(1000) Function229(d "func" 1 2 "str" "str")
 ARRAY(VARIANT) a.create(0)
 rep(1000)
	 x.x.Call("func" a)

 out "QM: 0x%X" GetCurrentThreadId
 x.Exec(_s.getfile(sf) "/?")
 int i
 for i 0 1000
	 out i
	 x.Exec(_s.getfile(sf) "str" 5)
	 0.01

x.AddCode(code)
PN
 x.x=0; PN
 out _i

 x.x.ExecFile(sf "/?")
 x.x.ExecFile(sf "/a /b ''c c c''")
 out "------"; d2.ExecFile(sf "/?")
 x.x.Exec(_s.getfile(sf) "test")
 PN


 x.AddCode(sf 4)
 out x.x.CallNoParams("Script.Test")
 PN

 x.AddCode(code)
 PN
 ARRAY(VARIANT) a.create(1); a[0]="test"
 out x.x.Call("Test.MsgBox" a)
 Q &qqqq
 ARRAY(VARIANT) aa.create(2); aa[0]=5; aa[1]=2
 out x.x.Call("Test.Add" aa)
  ARRAY(VARIANT) aa.create(1); aa[0]=999
  out x.x.Call("Test.Find" aa)
 PN
PO

 out x.x.Test(5)

 out x.x.Eval("Test() { return 5; }")
 out x.x.ExecFile(code "")

 opt waitmsg 1; wait 10
 1
