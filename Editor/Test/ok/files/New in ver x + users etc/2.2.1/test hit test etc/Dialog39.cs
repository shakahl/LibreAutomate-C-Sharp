\Dialog_Editor
 /exe
function# hDlg message wParam lParam
if(hDlg) goto messages

dll "qm.exe" SuperClass2 $newname $oldname newproc *oldproc

int+ __atomSTC __procSTC
if(!__atomSTC) __atomSTC=SuperClass2("SuperTC" "SysTabControl32" &WndProc22 &__procSTC)

if(!ShowDialog("Dialog39" &Dialog39)) ret

 BEGIN DIALOG
 0 "" 0x90C800CC 0x100 0 0 223 135 "Dialog"
 7 Button 0x54032000 0x0 56 22 48 14 "topmost"
 3 SuperTC 0x50000000 0x4 30 4 194 86 ""
 5 Button 0x50020007 0x0 18 0 158 76 "frame"
 11 Button 0x50020007 0x0 106 94 42 26 "frame2"
 8 Static 0x50000000 0x0 2 66 54 30 "Text"
 4 Button 0x50010000 0x4 108 0 104 58 "Button"
 1 Button 0x54030001 0x4 46 64 54 46 "OK"
 2 Button 0x54030000 0x4 148 118 68 14 "Cancel"
 6 Button 0x44032000 0x0 8 120 48 14 "hidden"
 9 Button 0x5C032000 0x0 60 120 48 14 "disabled"
 10 Button 0x50032000 0x0 112 100 30 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" "" ""
 3 SysTabControl32 0x50000000 0x4 2 4 216 86 ""

ret
 messages
sel message
	case WM_INITDIALOG
	 SetWindowTheme hDlg L"" L""
	
	int htb=id(3 hDlg)
	TCITEM ti.mask=WINAPI.TCIF_TEXT
	ti.pszText="A"
	SendMessage htb WINAPI.TCM_INSERTITEMA 0 &ti
	ti.pszText="B"
	SendMessage htb WINAPI.TCM_INSERTITEMA 1 &ti
	ti.pszText="C"
	SendMessage htb WINAPI.TCM_INSERTITEMA 2 &ti
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog39
 exe_file  $my qm$\Dialog39.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {3E63F6E1-00AC-4A72-BF1C-1D55E396EF87}
 END PROJECT
