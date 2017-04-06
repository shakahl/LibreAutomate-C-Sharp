 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "6 11 3 5"
__strt e6Fil c11No lb3Act c5Not

TO_FavSel wParam lb3Act "If file exists[]If folder exists[]If file or folder exists[]Search and get full path[]Expand special folder[]Unexpand special folder"

if(!ShowDialog("" &TO_FileIf &controls _hwndqm)) ret

str s n
int i=val(lb3Act)
e6Fil.rtrim("\")
e6Fil.S
if(c5Not=1) n="!"

sel i
	case 0 s=F"if({n}FileExists({e6Fil}))[][9]"
	case 1 s=F"if({n}FileExists({e6Fil} 1))[][9]"
	case 2 s=F"if({n}FileExists({e6Fil} 2))[][9]"
	case 3 s=F"_s.searchpath({e6Fil})[]if({n}_s.len)[][9]"
	case 4 s=F"_s.expandpath({e6Fil})"
	case 5 s=F"_s.expandpath({e6Fil} 2)"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 306 133 "If file exists"
 6 Edit 0x54030080 0x204 4 6 298 14 "File"
 7 Button 0x54032000 0x4 4 22 38 14 "File..."
 12 Button 0x54032000 0x4 44 22 38 14 "Folder..."
 8 Button 0x54032000 0x4 84 22 18 14 "SF" "Special folders"
 11 Button 0x54012003 0x0 106 22 34 12 "No SF" "Let the button give me normal path, not special folder name"
 3 ListBox 0x54230101 0x204 4 46 98 56 "Action"
 5 Button 0x54012003 0x4 106 90 30 13 "Not"
 1 Button 0x54030001 0x4 6 116 48 14 "OK"
 2 Button 0x54030000 0x4 56 116 48 14 "Cancel"
 4 Button 0x54032000 0x4 106 116 18 14 "?"
 10 QM_DlgInfo 0x54000000 0x20000 154 24 148 78 ""
 9 Static 0x54000010 0x20004 0 110 328 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\files.ico" "" 3)) ret wParam
sel message
	case WM_INITDIALOG
	goto g1
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7 sub_to.FileDialog hDlg 6 "iffdir"
	case 12 sub_to.FolderDialog hDlg 6
	case 8 sub_to.File_SF hDlg 6 "\"
	case 4 QmHelp "FileExists[]*[]*[]IDP_S_SEARCHPATH[]*[]*" TO_Selected(hDlg 3)
	 
	case LBN_SELCHANGE<<16|3
	 g1
	i=TO_Selected(hDlg 3)
	lpstr st
	sel i
		case 0 st="If exists, and is not folder or drive."
		case 1 st="If exists, and is folder or drive."
		case 2 st="If exists. Can be file, folder or drive."
		case 3 st="Can be file, folder or drive. If not full path, searches in My QM, QM, current folder, System32, Windows, PATH env. var., and in the registry. If finds, stores full path into str variable _s, else _s will be empty. Expands special folders, environment variables, . and .. parts."
		case 4 st="Expands $special folder$ or %environment variable% in path, eg $system$\file.exe to c:\windows\system32\file.exe. Does not search; the result does not depend on whether the file exists or not."
		case 5 st="Replaces part of path with QM special folder name, if possible, eg c:\windows\system32\file.exe to $system$\file.exe."
	SetDlgItemText(hDlg 10 st)
	TO_Show hDlg "5" i<4
ret 1

#opt nowarnings 1
