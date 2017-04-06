 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "7 16 5"
__strt qmt7 e16Pat c5Sho

SystemParametersInfo SPI_GETKEYBOARDCUES 0 &_i 0; if(_i) c5Sho=1

if(!ShowDialog("" &TO_Menu &controls _hwndqm)) ret

str s winVar winFind

qmt7.Win(winVar 0 winFind)
s=F"{winFind}men {e16Pat.S} {winVar}"
sub_to.Trim s " ''''"

 sub_to.TestDialog s
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 271 169 "Click menu"
 11 QM_DlgInfo 0x54000000 0x20000 0 0 272 22 "<>Sends menu item click message to its window. Works only with standard menus. Menu items can be identified by id or path. See also: <help #IDP_MEN>get menu item state</help>."
 6 Button 0x54032000 0x4 8 42 48 15 "Record"
 15 QM_DlgInfo 0x54000000 0x20000 66 44 196 13 "To record, you can use menu Tools -> Record Menu."
 3 Static 0x54000000 0x0 10 84 30 13 "Window"
 7 QM_Tools 0x54030000 0x10000 42 82 220 15 "1 64"
 9 Static 0x54000000 0x0 10 102 30 12 "Path"
 16 Edit 0x54030080 0x204 42 100 220 14 "Pat" "Path to the menu item.[]Example: &Edit\Select &All[]Add & before underlined characters."
 5 Button 0x54012003 0x4 10 118 202 13 "Show underlined characters in menus (system setting)"
 1 Button 0x54030001 0x4 4 150 48 14 "OK"
 2 Button 0x54010000 0x4 54 150 50 14 "Cancel"
 13 Button 0x54032000 0x4 106 150 16 14 "?"
 8 Static 0x54000010 0x20000 0 142 280 1 ""
 18 Button 0x54020007 0x0 4 68 264 66 "Use path"
 10 Button 0x54020007 0x0 4 28 264 32 "Use id"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\menu2.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	if(_portable) TO_Enable hDlg "5" 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 6 clo hDlg; men 33030 _hwndqm
	case 5 SystemParametersInfo SPI_SETKEYBOARDCUES 0 but(lParam) 3
	case 13 QmHelp "IDP_MEN"
ret 1
