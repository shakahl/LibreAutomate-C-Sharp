 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 8 5 23 7 19 17"
__strt e3Sou c8No e5Des cb23Ope cb7Exi lb19Fla cb17Act

TO_FavSel wParam cb17Act "Copy file[]Move or rename file"
cb23Ope="&Move, or move and rename[]Rename (dest. is new filename)"
cb7Exi="&Ask (show dialog)[]Create unique name[]Replace[]Error"
lb19Fla="No error message box[]No progress dialog[]*? match folders too"

if(!ShowDialog("" &TO_FileCopy &controls _hwndqm)) ret

str s o f
if(wParam and val(cb23Ope)) o="*"; else cb7Exi.SelS(o "<> + - !")

if lb19Fla!"000"
	f=" FOF_ALLOWUNDO|FOF_NOCONFIRMMKDIR"
	if(lb19Fla[0]='1') f+"|FOF_NOERRORUI"
	if(lb19Fla[1]='1') f+"|FOF_SILENT"
	if(lb19Fla[2]!'1') f+"|FOF_FILESONLY"

s=F"{iif(wParam `ren` `cop`)}{o} {e3Sou.S(`???`)} {e5Des.S(`???`)}{f}"

 sub_to.TestDialog s
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 300 191 "Copy file"
 3 Edit 0x54030080 0x204 8 16 284 14 "Sou" "Must be full path.[]It's possible to specify multiple files, read in Help."
 4 Button 0x54012000 0x4 8 32 36 14 "File..."
 21 Button 0x54032000 0x4 46 32 36 14 "Folder..."
 14 Button 0x54032000 0x4 84 32 16 14 "SF" "Special folders"
 8 Button 0x54012003 0x0 110 34 48 12 "No SF" "Let the button give me normal path, not special folder name"
 5 Edit 0x54030080 0x204 8 74 284 14 "Des"
 6 Button 0x54032000 0x4 8 90 36 14 "Folder..."
 15 Button 0x54032000 0x4 46 90 16 14 "SF" "Special folders"
 22 Static 0x54020000 0x4 110 92 60 12 "Operation"
 23 ComboBox 0x54230243 0x4 172 90 120 213 "Ope"
 11 Static 0x54020000 0x4 110 106 60 12 "If already exists"
 7 ComboBox 0x54230243 0x4 172 104 120 213 "Exi"
 19 ListBox 0x54230109 0x204 6 128 88 30 "Fla"
 1 Button 0x54030001 0x4 6 172 48 14 "OK"
 2 Button 0x54010000 0x4 56 172 50 14 "Cancel"
 16 Button 0x54032000 0x4 108 172 18 14 "?"
 17 ComboBox 0x44230243 0x4 138 172 96 213 "Act"
 20 Static 0x54000010 0x20004 0 164 314 2 ""
 12 Button 0x54020007 0x4 4 4 292 46 "Source file or folder"
 13 Button 0x54020007 0x4 4 62 292 61 "Destination folder or file"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 messages
if(sub_to.ToolDlgCommon(&hDlg "17[]$qm$\files2.ico" "" 3)) ret wParam
sel message
	case WM_INITDIALOG
	if(TO_Selected(hDlg 17 _s)) _s.setwintext(hDlg); else TO_Show hDlg "22 23" 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 sub_to.FileDialog hDlg 3 s "filesdir"
	case 21 sub_to.FolderDialog hDlg 3
	case 6 sub_to.FolderDialog hDlg 5
	case 14 sub_to.File_SF hDlg 3 "\"
	case 15 sub_to.File_SF hDlg 5
	case 16 QmHelp "IDP_COP"
	case CBN_SELENDOK<<16|23 TO_Show hDlg "7 11" !TO_Selected(lParam)
ret 1

#opt nowarnings 1
