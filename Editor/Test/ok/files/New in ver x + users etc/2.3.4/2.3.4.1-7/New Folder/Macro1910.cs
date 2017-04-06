/exe
out
int w=win("Keyboard Layout Creator 1.4 - 'Layout01 Description'" "WindowsForms10.Window.8.app.0.33c0d9d")
lef 5 5 child("" "*.BUTTON.*" w 0x800 "wfName=ckControl") 1 ;;check box 'Ctrl'

 GetProcessExename GetCurrentProcessId &_s; out _s

 out ProcessNameToId("firefox")
 out ProcessNameToId("svchost")
 out ProcessNameToId("svchost" 0 1)
 out ProcessNameToId("foxit reader")
 out ProcessNameToId("foxit reader" 0 1)
 ARRAY(int) a
 out ProcessNameToId("svchost" &a)
 out a.len
 out ProcessNameToId("Macro1909 reader")

 out _s.getwinexe("Foxit")
 out _s.getwinexe("QM Message")

 ARRAY(str) a
 EnumProcessesEx 0 &a 0
 out a
 ARRAY(int) a
 EnumProcessesEx &a 0 2
 out a.len

 out _s.encrypt(2|8)

 MES m.timeout=5
 mes "hhh" "" m

 web "www.quickmacros.com" 1|4 "" "" 0 _i
 outw _i


 BEGIN PROJECT
 main_function  Macro1910
 exe_file  $my qm$\Macro1910.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {1672D186-C41E-43EF-AE95-8F97C2F8ED97}
 END PROJECT
