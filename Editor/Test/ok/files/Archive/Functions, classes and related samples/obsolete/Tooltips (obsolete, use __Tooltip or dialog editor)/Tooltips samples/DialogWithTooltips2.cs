/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile __CToolTip

str controls = "4"
str e4
if(!ShowDialog("" &DialogWithTooltips2 &controls 0 0 0 0 0 0 0 "qm.exe")) ret
 if(!ShowDialog("" &DialogWithTooltips2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 6 48 14 "Button"
 4 Edit 0x54030080 0x200 62 6 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010803 "" ""

ret
 messages
CToolTip-- tt ;;CToolTip variables usually can be declared with thread scope (- or --). If thread can have multiple instances of the same dialog, use window scope (SetProp/GetProp). Don't use global and local.
sel message
	case WM_INITDIALOG
	tt.Create(hDlg)
	tt.AddTool(hDlg 3 "button")
	tt.AddTool(hDlg 4 "edit")
	 or
	 tt.AddTools(hDlg "3 button[]4 edit")
	
	str title="dialog with tooltips"
	int hi=GetWindowIcon(hDlg)
	SendMessage(tt.m_htt TTM_SETTITLE hi title)
	DestroyIcon hi
	
	ret 1
	
	case WM_DESTROY
	tt.Destroy
	
	case WM_SETCURSOR
	tt.OnWmSetcursor(wParam lParam)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  DialogWithTooltips2
 exe_file  $my qm$\DialogWithTooltips2.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {08D1CF1D-8771-4AD6-9717-9CC3CEC340E2}
 END PROJECT
