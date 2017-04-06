 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 7 5 13"
__strt e3Fil c7No c5Mov c13mat

c5Mov=1

if(!ShowDialog("" &TO_FileDelete &controls _hwndqm)) ret

str s=F"del{`-`+(c5Mov=1)} {e3Fil.S(`???`)}"

if(c13mat=1) s+" FOF_NOCONFIRMATION|FOF_SILENT|FOF_ALLOWUNDO|FOF_NOERRORUI"

 sub_to.TestDialog s
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 299 121 "Delete file or folder"
 3 Edit 0x54030080 0x204 8 16 284 14 "Fil" "Must be full path.[]It's possible to specify multiple files, read in Help."
 4 Button 0x54012000 0x4 8 32 36 14 "File..."
 21 Button 0x54032000 0x4 46 32 36 14 "Folder..."
 14 Button 0x54032000 0x4 84 32 16 14 "SF" "Special folders"
 7 Button 0x54012003 0x0 112 34 48 12 "No SF" "Let the button give me normal path, not special folder name"
 5 Button 0x54012003 0x4 8 60 86 13 "Move to Recycle Bin"
 13 Button 0x54012003 0x0 8 74 86 13 "*? match folders too"
 1 Button 0x54030001 0x4 6 102 48 14 "OK"
 2 Button 0x54010000 0x4 56 102 50 14 "Cancel"
 6 Button 0x54032000 0x4 108 102 18 14 "?"
 12 Button 0x54020007 0x4 3 5 294 46 "File or folder"
 11 Static 0x54000010 0x20004 0 94 318 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\del.ico" "" 3)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 sub_to.FileDialog hDlg 3 "filesdir"
	case 21 sub_to.FolderDialog hDlg 3
	case 14 sub_to.File_SF hDlg 3 "\"
	case 6 QmHelp "IDP_DEL"
ret 1

#opt nowarnings 1
