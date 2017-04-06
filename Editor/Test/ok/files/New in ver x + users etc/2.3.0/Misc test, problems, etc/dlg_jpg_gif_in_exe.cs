 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
 sb3="$my pictures$\Screensaver\toolbar1 24.bmp"
 sb3=":5 $my pictures$\Screensaver\toolbar1 24.bmp"
sb3=":6 $my pictures$\Screensaver\bjork.jpg"
 sb3=":7 $my pictures$\Screensaver\fleur.gif"
 sb3=":8 $my pictures$\Screensaver\bjorkjpeg.jpeg"
 sb3=":9 $my pictures$\Screensaver\tsw88x31sdt.gif"
if(!ShowDialog("" &dlg_jpg_gif_in_exe &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x5400100E 0x20000 6 6 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dlg_jpg_gif_in_exe
 exe_file  $my qm$\dlg_jpg_gif_in_exe.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {A44072D7-4E34-4155-AD5F-EE68C648A98B}
 END PROJECT
