 /exe
\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Button 0x54032000 0x0 8 8 48 14 "1"
 4 Button 0x54032000 0x0 64 8 48 14 "0"
 5 Button 0x54032000 0x0 120 8 48 14 "Get"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_SETTINGCHANGE
	out "%i %i" wParam lParam
	TO_TooltipOsd F"{wParam} {lParam}" 0 0 0 0 0 id(5 hDlg)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case 3 ;;1
	 SystemParametersInfo(SPI_SETSCREENREADER 1 0 SPIF_SENDCHANGE)
	SystemParametersInfo(SPI_SETSCREENREADER 1 0 0)
	
	case 4 ;;0
	 SystemParametersInfo(SPI_SETSCREENREADER 0 0 SPIF_SENDCHANGE)
	SystemParametersInfo(SPI_SETSCREENREADER 0 0 0)
	
	case 5 ;;Get
	SystemParametersInfo(SPI_GETSCREENREADER 0 &_i 0)
	TO_TooltipOsd F"{_i}" 0 0 0 0 0 id(5 hDlg)
	
ret 1

 BEGIN PROJECT
 main_function  dialog_test_WM_SETTINGCHANGE
 exe_file  $my qm$\Dialog170.qmm
 flags  6
 guid  {84E49964-A194-4556-B15C-3DA4E87E0C56}
 END PROJECT
