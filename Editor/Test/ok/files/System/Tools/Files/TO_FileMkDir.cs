 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 14 5"
__strt e3Pat c14No e5Nam

if(!ShowDialog("" &TO_FileMkDir &controls _hwndqm)) ret

str s
if(e5Nam.len) s=F"mkdir {e5Nam.S} {e3Pat.S}"; sub_to.Trim s " ''''"
else s=F"mkdir {e3Pat.S}"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 291 127 "Create folder"
 6 Static 0x54020000 0x4 4 46 24 12 "Path"
 3 Edit 0x54010080 0x204 30 44 256 14 "Path"
 4 Button 0x54012000 0x4 30 60 50 14 "Browse..."
 7 Button 0x54032000 0x4 82 60 16 14 "SF" "Special folders"
 14 Button 0x54012003 0x0 104 62 48 12 "No SF" "Let the button give me normal path, not special folder name"
 11 QM_DlgInfo 0x54000000 0x20000 0 0 292 36 "Creates folder if does not exist. Also creates parent folders if don't exist.[][]You can specify full path in 'Path' field, and leave 'Name' field empty.[]Or specify parent folder in 'Path' field and new folder name in 'Name' field."
 9 Static 0x54020000 0x4 4 82 24 12 "Name"
 5 Edit 0x54030080 0x204 30 80 122 14 "Name"
 1 Button 0x54030001 0x4 4 108 48 14 "OK"
 2 Button 0x54010000 0x4 54 108 50 14 "Cancel"
 8 Button 0x54032000 0x4 106 108 18 14 "?"
 10 Static 0x54000010 0x20004 0 100 314 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\folder.ico" "3" 1)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 sub_to.FolderDialog hDlg 3
	case 7 sub_to.File_SF hDlg 3
	case 8 QmHelp "IDP_MKDIR"
ret 1

#opt nowarnings 1
