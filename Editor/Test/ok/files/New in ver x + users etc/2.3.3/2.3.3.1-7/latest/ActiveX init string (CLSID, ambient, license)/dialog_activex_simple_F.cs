 /exe
\Dialog_Editor
 out
 #opt nowarnings 1
typelib MSACAL {8E27C92E-1264-101C-8A2F-040224009C02} 7.0
typelib test_licensed_control {46E38ED9-4630-4EE6-A5F8-69E310FD84CB} 6.0

str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 ActiveX 0x54030000 0x0 128 0 96 48 "SHDocVw.WebBrowser"
 5 ActiveX 0x54030000 0x0 128 50 96 48 "SHDocVw.WebBrowser ambient:{&WebBrowser_AmbientProc},{0x80}"
 10 ActiveX 0x54030000 0x0 0 0 94 48 "{{8E27C92B-1264-101C-8A2F-040224009C02}"
 11 ActiveX 0x54030000 0x0 0 50 94 48 "MSACAL.Calendar {{8E27C92B-1264-101C-8A2F-040224009C02}"
 15 ActiveX 0x54030000 0x0 6 100 96 48 "test_licensed_control.UserControl1 {{D45F8166-E09C-4483-8C76-91122CDCA5E4} data:8EFB7E83010EA8D88985DC4D89DC041302359A260DAECFEC04"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

str controls = "4 5"
str ax4SHD ax5SHD
ax4SHD="http://www.quickmacros.com/test/test.html"
ax5SHD=ax4SHD
if(!ShowDialog(dd 0 &controls)) ret

 BEGIN PROJECT
 main_function  dialog_activex_simple_F
 exe_file  $my qm$\Dialog99.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {47896252-9E0E-4F17-AF48-2C0B1D55B14E}
 END PROJECT
