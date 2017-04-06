 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 5 6 11 13 15 16"
__strt e4Fil c5Fol c6No lb11Get lb13Att cb15Tim c16Tim

lb11Get="Attributes[]Size[]Time modified[]Time created[]Time accessed[]Is folder[]Data"
lb13Att=__S_FILE_ATTR
cb15Tim="&UTC time[]Local time"

if(!ShowDialog("" &TO_FileInfo &controls _hwndqm)) ret

__strt& g=lb11Get
if(g="0000000") g[0]='1'
e4Fil.rtrim("\"); e4Fil.S

str s
__strt vd vAttr vSize(0) vt1(0) vt2(0) vt3(0) vData
ARRAY(str) aa; int i
if(g[5]='1') lb13Att[3]='1'
if(lb13Att.LbSelectedItemsToNames(aa __S_FILE_ATTR "" 1)) g[0]='1'
if(g[1]='1') s+F"{vd.VD(`-i long size` vSize)}[]"
if(g[2]='1') s+F"{vd.VD(`-i DateTime tm` vt1)}[]"
if(g[3]='1') s+F"{vd.VD(`-i DateTime tc` vt2)}[]"
if(g[4]='1') s+F"{vd.VD(`-i DateTime ta` vt3)}[]"
if(g[0]='1' or g[5]='1') s+F"{vd.VD(`-i int attr` vAttr)}="

if s.len ;;else need only file data
	s+F"FileGetAttributes({e4Fil} {vSize} {vt1} {vt2} {vt3}"
	sub_to.Trim(s " 0 0 0 0")
	s+") ;;err out _error.description; ret[]"
	for i 0 aa.len
		str& ra=aa[i]; if(!ra.len) continue
		s+F"if({vAttr}&FILE_ATTRIBUTE_{ra}) out ''{ra}''[]"
	if val(cb15Tim)=1
		if(vt1!0) s+F"{vt1}.UtcToLocal[]"
		if(vt2!0) s+F"{vt2}.UtcToLocal[]"
		if(vt3!0) s+F"{vt3}.UtcToLocal[]"
	if c16Tim=1
		if(vt1!0) s+F"_s={vt1}.ToStr(4); out _s[]"
		if(vt2!0) s+F"_s={vt2}.ToStr(4); out _s[]"
		if(vt3!0) s+F"_s={vt3}.ToStr(4); out _s[]"
if(g[6]='1') s+F"{vd.VD(`str data` vData)}.getfile({e4Fil});; err ...[]"
s.rtrim

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 299 161 "Get file info"
 3 Static 0x54020000 0x4 4 6 16 10 "File"
 4 Edit 0x54030080 0x204 4 20 292 14 "Fil" "Full path"
 5 Button 0x54012003 0x0 36 4 42 13 "Folder"
 7 Button 0x54032000 0x4 80 4 44 14 "Browse..."
 8 Button 0x54032000 0x4 126 4 16 14 "SF" "Special folders"
 6 Button 0x54012003 0x0 152 4 40 13 "No SF" "Let the button give me normal path, not special folder name"
 10 Static 0x54000000 0x0 4 42 48 10 "Get"
 11 ListBox 0x54230109 0x200 4 52 102 80 "Get" "Select one or several"
 12 Static 0x54000000 0x0 126 42 60 10 "Test attributes"
 13 ListBox 0x54230109 0x200 126 52 102 80 "Att" "Select one or several"
 15 ComboBox 0x54230243 0x0 244 104 52 213 "Tim"
 16 Button 0x54012003 0x0 244 120 58 12 "Time string"
 1 Button 0x54030001 0x4 4 142 48 14 "OK"
 2 Button 0x54030000 0x4 54 142 48 14 "Cancel"
 41 Button 0x54032000 0x4 104 142 16 14 "?"
 9 Static 0x54000010 0x20000 0 136 336 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040101 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\files.ico" "4" 1)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7 if(but(5 hDlg)) sub_to.FolderDialog hDlg 4; else sub_to.FileDialog hDlg 4 "filesdir"
	case 8 sub_to.File_SF hDlg 4 "\"
	case 41 QmHelp "FileGetAttributes"
ret 1

#opt nowarnings 1
