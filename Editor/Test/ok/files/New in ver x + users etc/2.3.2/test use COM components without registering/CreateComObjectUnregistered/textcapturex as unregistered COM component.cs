/exe
SetCurDir "G:\Program Files\Deskperience\ScreenScraper Studio" ;;without this, possible error "The specified module could not be found", because the dll is statically linked with other dlls in same folder without specifying full path.
typelib TCaptureXLib "G:\Program Files\Deskperience\ScreenScraper Studio\TCaptureX.dll"
TCaptureXLib.TextCaptureX t=CreateComObjectUnregistered("G:\Program Files\Deskperience\ScreenScraper Studio\TCaptureX.dll" uuidof(TCaptureXLib.TextCaptureX) uuidof(TCaptureXLib.ITextCaptureX))
int hwnd x y cx cy
if(t.CaptureInteractive(hwnd x y cx cy)) ret
str s=t.GetTextFromRect(hwnd x y cx cy)
out s

 BEGIN PROJECT
 main_function  Macro1366
 exe_file  $my qm$\Macro1366.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {1B5A1266-677B-4C94-B3B4-B1800AD95CE2}
 END PROJECT
