 /exe 1
 \Dialog_Editor

 BEGIN PROJECT
 main_function  user_exe
 exe_file  $my qm$\user_exe.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {C1D22733-FE87-45DA-B493-30244CBF8983}
 END PROJECT


str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 8 56 14 "send keys to"
 4 Edit 0x54030080 0x200 72 8 96 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "4"
str e4
e4="Word"
 e4="Calculator"
if(!ShowDialog(dd &sub.DlgProc &controls 0 0 0 0 0 -1 1)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	act _s.getwintext(id(4 hDlg))
	key "12"
ret 1
