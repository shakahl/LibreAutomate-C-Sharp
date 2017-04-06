/exe
out

;captures text of user selected rectangle

typelib TCaptureXLib {92657C70-D31B-4930-9014-379E3F6FB91A} 1.1
TCaptureXLib.TextCaptureX t._create
int hwnd x y cx cy
if(t.CaptureInteractive(hwnd x y cx cy)) ret
str s=t.GetTextFromRect(hwnd x y cx cy)
out s


 BEGIN PROJECT
 main_function  Macro335
 exe_file  $my qm$\Macro335.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {E138E4CA-ACB3-461A-AA03-61C11CA840B2}
 END PROJECT
