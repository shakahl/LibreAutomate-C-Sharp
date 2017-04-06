/exe
ref JAB "__jab_api" 1

shutdown -6 0 "eat_cpu"
 rep(2) mac "eat_cpu"
0.1

 int+ g_jab_inited
 if !g_jab_inited
	  RunConsole2("$Program Files$\Java\jre7\bin\jabswitch -enable") ;;once
	 JAB.initializeAccessBridge
	 g_jab_inited=1

dll- user32 #ChangeWindowMessageFilter message dwFlag
if(_winnt>=6) ChangeWindowMessageFilter(RegisterWindowMessage("AccessBridge-FromJava-Hello") 1)

int+ g_jab_inited
 if !g_jab_inited
	 SetThreadPriority GetCurrentThread THREAD_PRIORITY_LOWEST
	JAB.initializeAccessBridge
	 g_jab_inited=1
	  0.1
	  out JAB.GetVersionInfo
	 0
	 opt waitmsg 1
	 PF
	  0.02
	  wait 1 P 15
	  PN
	 rep(10) 0.003
	 PN; PO
	 SetThreadPriority GetCurrentThread THREAD_PRIORITY_NORMAL
	
	 PF
	MSG m
	rep 4
		GetMessage(&m 0 0 0)
		PN
		OutWinMsg m.message m.wParam m.lParam
		DispatchMessage &m
	 PO
	 0
	 opt waitmsg 1; 0.1
 1

 int w=win("Stylepad" "SunAwtFrame")
int w=win("SwingSet2" "SunAwtFrame")
 if !w
	 run "$program files$\Java\jdk1.7.0_21\demo\jfc\SwingSet2\SwingSet2.jar"
	 w=wait(0 WA win("SwingSet2" "SunAwtFrame"))
	 1
outw w

if(!JAB.IsJavaWindow(w)) end "IsJavaWindow"
int vmID aCtx
 if(!JAB.GetAccessibleContextFromHWND(w &vmID &aCtx)) end "GetAccessibleContextFromHWND"
 out "%i %i" vmID aCtx

 JAB.SetMouseEntered &AccessBridge_MouseEnteredFP
 opt waitmsg 1
 10

shutdown -6 0 "eat_cpu"

 BEGIN PROJECT
 main_function  test Java Access Bridge
 exe_file  $my qm$\test Java Access Bridge.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {72277D0F-20BD-440F-BA95-69F6CA42DF68}
 END PROJECT
