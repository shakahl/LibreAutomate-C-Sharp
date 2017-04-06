 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 5 8 9 10 11 12 13 14 15"
str c4Che e5 e8 e9edi rea10 cb11 cb12 lb13 si14 sb15
e5="test"
e8="password"
rea10="rich"
cb11="&cb[]bb"
cb12="&cb[]bb"
lb13="&lb[]bb"
si14="$qm$\mouse.ico"
sb15="$qm$\il_de.bmp"
if(!ShowDialog("dlg_controls" &dlg_controls &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 0 0 48 14 "Button"
 4 Button 0x54012003 0x0 50 2 48 12 "Check"
 5 Edit 0x54030080 0x200 102 0 96 14 ""
 6 Static 0x54000000 0x0 0 16 48 12 "Text"
 8 Edit 0x54030020 0x200 158 16 40 14 ""
 9 Edit 0x54230844 0x20000 0 32 94 14 "edit readonly"
 10 RichEdit20A 0x54233044 0x200 98 32 96 14 ""
 11 ComboBox 0x54230243 0x0 0 48 96 213 ""
 12 ComboBox 0x54230242 0x0 98 48 96 213 ""
 13 ListBox 0x54230101 0x200 2 64 86 18 ""
 14 Static 0x54000003 0x0 4 102 16 16 ""
 15 Static 0x5400000E 0x0 36 102 16 16 ""
 17 msctls_hotkey32 0x54030000 0x200 4 86 82 12 ""
 18 SysTreeView32 0x54030000 0x0 94 70 42 28 ""
 16 Static 0x54000010 0x20000 96 66 52 1 ""
 7 Button 0x54020007 0x0 50 16 102 12 "Group"
 END DIALOG
 DIALOG EDITOR: "" 0x2030301 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	_s="TV"; _s.setwintext(id(18 hDlg))
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dlg_controls
 exe_file  $my qm$\dlg_controls.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4B6821A4-3A95-4E70-BE89-FB7AC0054C93}
 END PROJECT
