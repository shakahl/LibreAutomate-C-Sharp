 \Dialog_Editor
function# hDlg message wParam lParam ;;wParam: 0 run prog, 1 open file, 2 open folder.  lParam can be window handle of program to run
if(hDlg) goto messages

str controls = "3 18 1308 1002 1303 21 14 1309 17 9 7 10 19 29 5"
__strt e3Pat c18No c1308Sho e1002Par e1303Dir cb21Ver cb14Sta cb1309Adm lb17Opt qmt9 cb7Win cb10Win c19Get e29mon cb5Act

TO_FavSel wParam cb5Act "Run program[]Open file[]Open folder"
cb7Win="&Just activate[]Do nothing[]Run anyway"
cb10Win="&Run and wait for active[]Run and don't wait for active"
cb14Sta="&Normal[]Maximized[]Minimized[]Min. inactive[]Inactive[]Hidden"
lb17Opt="Don't show error message box[]Wait for input idle[]Wait for exit[]"
cb1309Adm="&Normal[]Administrator[]Admin if admin account[]As QM"

if(!ShowDialog("" &TO_FileRun &controls _hwndqm 0 0 0 lParam)) ret

int i j f
str s winVar sHwnd p1(" ") p2
__strt vd

cb21Ver.CbItem
qmt9.Win(winVar "''''")
e29mon.NE; if(e29mon.len) c19Get=1

if(c19Get=1) s=vd.VD("int w[]" sHwnd)

sel(val(cb14Sta)) case 1 f=SW_SHOWMAXIMIZED; case 2 f=SW_SHOWMINIMIZED; case 3 f=SW_SHOWMINNOACTIVE; case 4 f=SW_SHOWNOACTIVATE; case 5 f=16
f|val(cb1309Adm)&3<<16 ;;admin
if(lb17Opt[0]=='1') f|0x100 ;;noerr
if(lb17Opt[1]=='1') f|0x200 ;;wait idle
if(lb17Opt[2]=='1') ;;wait exit
	f|0x400
	s+vd.VD("int exitCode="); p1="("; p2=")"
	if(e29mon.len) out "Warning: 'move to monitor' is incompatible with 'wait for exit'."
if(winVar!="''''")
	f|val(cb7Win)<<12
	if(!val(cb10Win)) f|0x800
if(cb21Ver~"properties") f|0x40000

s+F"run{p1}{e3Pat.S(`???`)} {e1002Par.S} {cb21Ver.S} {e1303Dir.S} {vd.Flags(f 8)} {winVar} {sHwnd}{p2}"
sub_to.Trim s " '''' '''' '''' 0 '''' "

if(e29mon.len) s+F"[]MoveWindowToMonitor {sHwnd} {e29mon}"

 sub_to.TestDialog s
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 300 204 "Run"
 3 Edit 0x54010080 0x204 4 4 292 14 "Pat" "Full path or filename.[]If filename, use the Find button to test whether will find it.[]You can drag and drop a file here, as well as in other dialogs and QM editor.[]You can select a window from the list to fill this field if empty."
 4 Button 0x54012000 0x4 4 20 48 14 "Browse..."
 22 Button 0x54032000 0x0 52 20 48 14 "Menu" "Select from a menu"
 31 Button 0x54032000 0x4 100 20 16 14 "SF" "Special folders"
 18 Button 0x54012003 0x0 136 20 40 14 "No SF" "Let the button give me normal path, not special folder name"
 1308 Button 0x44012003 0x4 180 20 48 14 "Shortcut" "For shortcuts use .lnk file, not target file"
 16 Button 0x54032000 0x4 248 20 48 14 "Find" "Test whether the file can be found.[]For example, you can test whether will find file if used just filename, like notepad.exe."
 1001 Static 0x44020000 0x4 4 42 36 12 "Param."
 1002 Edit 0x44010080 0x204 42 40 238 14 "Par" "Command line"
 1003 Button 0x44032000 0x4 280 40 16 14 "..."
 1302 Static 0x44020000 0x4 4 58 36 12 "Directory"
 1303 Edit 0x44010080 0x204 42 56 196 14 "Dir" "Default 'current directory'.[]If empty, will be used current directory of QM."
 1305 Button 0x44032000 0x4 238 56 16 15 "..."
 1306 Button 0x44032000 0x4 254 56 16 14 "SF" "Special folders"
 1304 Button 0x44032000 0x4 270 56 26 14 "Home" "Use program's directory"
 20 Static 0x54000000 0x0 4 76 36 13 "Verb"
 21 ComboBox 0x54230242 0x0 42 74 96 213 "Verb" "One of actions that you see in the right-click menu of the file"
 11 Static 0x54020000 0x4 4 92 36 13 "Show"
 14 ComboBox 0x54210243 0x4 42 90 96 213 "State" "Window show state.[]Most programs ignore it."
 1310 Static 0x54000000 0x0 4 108 36 13 "Run as"
 1309 ComboBox 0x54230243 0x0 42 106 96 213 "Adm" "Windows Vista/7/8/10 UAC integrity level"
 17 ListBox 0x54230109 0x204 180 74 116 44 "Opt" "Flags"
 8 Static 0x54020000 0x4 4 130 36 12 "Window"
 9 QM_Tools 0x54030000 0x10000 42 130 254 14 "1 0x1C1"
 12 Static 0x5C020000 0x4 4 146 36 28 "If exists[][]Else"
 7 ComboBox 0x5C230243 0x4 42 146 96 213 "Win"
 10 ComboBox 0x5C230243 0x4 42 160 96 213 "Win"
 19 Button 0x5C012003 0x0 202 146 94 12 "Get handle"
 6 Static 0x5C000000 0x0 202 162 58 13 "Move to monitor"
 29 Edit 0x5C030080 0x204 262 160 34 14 "mon" "1-based monitor index"
 1 Button 0x54030001 0x4 4 186 48 14 "OK"
 2 Button 0x54010000 0x4 54 186 48 14 "Cancel"
 33 Button 0x54032000 0x4 104 186 16 14 "?"
 15 Button 0x54032000 0x0 122 186 34 14 "? More"
 5 ComboBox 0x44230243 0x4 200 186 96 213 "Act"
 13 Static 0x54000010 0x20004 4 123 294 1 ""
 26 Static 0x54000010 0x20004 0 178 304 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "0" "(0 3) (1 3)" ""

 messages
sel message
	case WM_INITDIALOG
	QmRegisterDropTarget hDlg 0 48
	DT_MouseWheelRedirect
	i=TO_Selected(hDlg 5 s)
	s.setwintext(hDlg)
	DT_Page hDlg i "(0 3) (1 3)"
	i=DT_GetParam(hDlg); if(i) SendMessage id(9 hDlg) __TWM_DRAGDROP 4 i ;;from Ctrl+Shift+Alt+W menu, param is window handle
	
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP TO_DropFiles hDlg +lParam "3 1002- 1303" 3 1002
	
	case WM_USER+7
	sel(TO_Selected(hDlg 5)) case 0 s="$qm$\run.ico"; case 1 s="$qm$\files3.ico"; case 2 s="$qm$\folder_open.ico"
	ret TO_FavRet(lParam 5 s)
	
	case __TWN_WINDOWCHANGED ;;window control text changed
	TO_Enable hDlg "12 7 10 19 6 29" GetWindowTextLengthW(lParam)!=0
	if(!wParam) ret ;;else wParam is handle of window selected from list; get its exe
	str-- t_lastExe
	s.getwintext(id(3 hDlg)); if(s.len and s!=t_lastExe) ret
	_i=sub_sys.GetWindowsStoreAppId(wParam s 3); if(!_i) ret
	if(_i=2) sub_to.File_UnexpandPathIfNeed hDlg s
	t_lastExe=s
	EditReplaceSel hDlg 3 s 1
ret
 messages2
int ctrlid(wParam&0xFFFF) tool=TO_Selected(hDlg 5)
sel wParam
	case 4
	sel tool
		case 0 sub_to.FileDialog hDlg 3 "rundir" "C:" ".exe[]*.exe[].bat[]*.bat[].com, .cmd[]*.com;*.cmd[]Screen savers[]*.scr[]All Files[]*.*[]" "exe" 0 1002
		case 1 sub_to.FileDialog hDlg 3 "opendir"
		case 2 sub_to.FolderDialog hDlg 3 "" 2
	
	case 1003 if(sub_to.FileDialog(hDlg 0 "opendir" "" "" "" s)) TO_SetText s.expandpath hDlg 1002
	case 1305 sub_to.FolderDialog hDlg 1303
	case 1304 TO_SetText "*" hDlg 1303
	case 31 sub_to.File_SF hDlg 3 "\"
	case 1306 sub_to.File_SF hDlg 1303
	
	case 16 ;;Find
	s.getwintext(id(3 hDlg))
	if(s.len) s.searchpath(s); sub_sys.MsgBox hDlg iif(s.len s "Not found.") "" "i"
	
	case 22 sub_to.File_FileMenu hDlg 3 0 0 1002 tool=2
	case [17,27] TO_Check hDlg "25" 0
	case 33 QmHelp "IDP_RUN"
	
	case CBN_DROPDOWN<<16|21 goto gVerb
	
	case LBN_SELCHANGE<<16|17 TO_LbUnselect lParam 2 "1"; TO_LbUnselect lParam 1 "2"
	
	case IDOK ret sub_to.CanInsertStatement(hDlg)
	
	case 15 goto gMore
ret 1

 gVerb
str s1 s2; ARRAY(str) a
TO_CBFill lParam "&[]properties"
if(tool=2) s1="Folder"
else
	s.getwintext(id(3 hDlg))
	i=findcr(s '.')
	if(i>=0) rget(s1 "" s.get(s i) HKEY_CLASSES_ROOT)
	s2="*"
for j 0 2
	if(j) s1=s2
	if(!s1.len) continue
	s1+"\shell"; s="&"
	if(RegGetSubkeys(a s1 HKEY_CLASSES_ROOT))
		for(i 0 a.len) CB_Add lParam a[i]
ret

 gMore
s=
 <b>Other "run" functions</b>
;
 <help>RunConsole2</help> - run a command line program and get its output.
;
 <help>RunAs</help> - run a program as other user, eg as administrator on a user account.
;
 <help>StartProcess</help> - run a program with specified UAC integrity level. Supports more levels than run().
;
 Also look in the 'file' <help #IDP_CATEGORIES>category</help>.
QmHelp s 0 6
ret

#opt nowarnings 1
