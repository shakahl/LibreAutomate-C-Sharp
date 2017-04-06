\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_toolbar" &dialog_toolbar)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 319 197 "Form"
 1 Button 0x54030001 0x4 120 166 48 14 "OK"
 2 Button 0x54030000 0x4 170 166 48 14 "Cancel"
 3 ToolbarWindow32 0x5400000C 0x0 0 0 294 30 ""
 4 ToolbarWindow32 0x5400000C 0x0 0 30 294 18 ""
 5 ToolbarWindow32 0x5400000C 0x0 0 48 294 18 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010805 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	int styles=WINAPI.TBSTYLE_FLAT
	DT_TbAddButtons id(3 hDlg) 1001 "one[]two[]-[]three[]four" "$qm$\il_qm.bmp" styles|WINAPI.CCS_NODIVIDER
	DT_TbAddButtons id(4 hDlg) 1101 "[][]-60[][][]" "$qm$\il_qm.bmp" styles
	DT_TbAddButtons id(5 hDlg) 1201 "one[]two[]three" "shell32.dll,1[]shell32.dll,2[]shell32.dll,3" styles|WINAPI.TBSTYLE_LIST 1
	ret 1
	
	case WM_DESTROY
	DT_DeleteData(hDlg)
	DT_TbOnDestroy id(3 hDlg)
	DT_TbOnDestroy id(4 hDlg)
	DT_TbOnDestroy id(5 hDlg)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 1001 mes "button 1"
	case 1002 mes "button 2"
	case 1003 mes "button 3"
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
