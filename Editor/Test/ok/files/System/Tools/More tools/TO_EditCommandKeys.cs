 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

sub_sys.PortableWarning
str controls = "3 5 4"
str qmg3x e5 c4All
e5=
 Some keyboards have extra keys, such as Back, Mute, Calculator. You can change or disable actions assigned to them. It is a Windows feature and does not depend on whether QM is running.
;
 Actions:
   Disable - disable any action.
   Run - run specified program. Value examples: calc.exe, "c:\folder\program.exe" /command.
   Association - run program that opens specified file type or protocol. Value examples: .txt, mailto.
   RegisteredApp - run program registered for specified value. Select value from dropdown list.
   Default or empty - default action. It depends on currently active window, and may be no action.
;
 Priority of actions:
   1. Disable/Run for current user.
   2. Disable/Run for all users.
   3. Association for current user.
   4. Association for all users.
   5. RegisteredApp for current user.
   6. RegisteredApp for all users.
   7. Default action.
;
 You should disable keys that you use as triggers. Not necessary if low level hook (Options -> Triggers).

if(!ShowDialog("" &TO_EditCommandKeys &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 347 234 "Edit Application Command Keys"
 3 QM_Grid 0x54210001 0x200 0 0 348 126 "0x3,1,0,2,0x0[]Key,25%,,[]Action,18%,1,[]Value,55%,1,"
 5 QM_DlgInfo 0x54000000 0x20000 0 132 348 77 ""
 4 Button 0x54012003 0x0 6 217 48 13 "All Users"
 1 Button 0x54030001 0x4 68 217 48 14 "Apply"
 2 Button 0x54030000 0x4 120 217 48 14 "Close"
 6 Button 0x54032000 0x0 296 217 48 14 "Restore..."
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\keyboard.ico")) ret wParam
sel message
	case WM_INITDIALOG
	sub.Work 0 hDlg
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK ;;Apply
	sub.Work 1 hDlg
	ret
	
	case 4 ;;All Users
	if(but(lParam) and !IsUserAdmin) but- lParam; mes "QM must run as administrator." "" "!"; ret
	sub.Work 0 hDlg
	
	case 6 ;;Restore
	sub.Work 2 hDlg
	
ret 1
 messages3
NMHDR* nh=+lParam
GRID.QM_NMLVDATA* cd=+nh
DlgGrid g.Init(hDlg 3)
sel nh.idFrom
	case 3
	NMLVDISPINFO* di=+nh
	sel nh.code
		case GRID.LVN_QG_COMBOFILL
		sel cd.subitem
			case 1
			TO_CBFill cd.hcb "Disable[]Run[]Association[]RegisteredApp[]Default"
			case 2
			sel g.CellGet(cd.item 1) 1
				case "RegisteredApp"
				ARRAY(str) ask; if(RegGetSubkeys(ask "Software\Clients")) _s=ask; TO_CBFill cd.hcb _s

#opt nowarnings 1


#sub Work
function action hDlg ;;action: 0 init, 1 apply, 2 restore default


int i hGrid admin hive
str s sv sk skAppKey skAppKeyBackup
RegKey rk
ICsv c._create

hGrid=id(3 hDlg)
admin=but(4 hDlg)
hive=iif(admin HKEY_LOCAL_MACHINE HKEY_CURRENT_USER)
skAppKey="Software\Microsoft\Windows\CurrentVersion\Explorer\AppKey"
skAppKeyBackup=F"{skAppKey} QM Backup"

sel action
	case 0 ;;init
	c.FromString("Browser Back,,[]Browser Forward[]Browser Refresh[]Browser Stop[]Browser Search[]Browser Favorites[]Browser Home[]Volume Mute[]Volume Down[]Volume Up[]Media Next[]Media Prev[]Media Stop[]Media Play/Pause[]Launch Mail[]Launch Media[]Launch App1[]Launch App2")
	for i 0 18
		sk=F"{skAppKey}\{i+1}"
		if(rget(s "ShellExecute" sk hive))
			int isDisabled=s="<disable>"
			c.Cell(i 1)=iif(isDisabled "Disable" "Run")
			if(isDisabled) continue
		else if(rget(s "Association" sk hive)) c.Cell(i 1)="Association"
		else if(rget(s "RegisteredApp" sk hive)) c.Cell(i 1)="RegisteredApp"
		else continue
		c.Cell(i 2)=s
	c.ToQmGrid(hGrid)
	TO_Enable hDlg "6" sub_to.RegKeyExists(skAppKeyBackup hive)
	
	case 1 ;;apply
	 backup first time
	if(!sub_to.RegKeyExists(skAppKeyBackup hive))
		if(RegCreateKeyExW(hive @skAppKeyBackup 0 0 0 KEY_ALL_ACCESS 0 &rk 0) or (sub_to.RegKeyExists(skAppKey hive) and SHCopyKeyW(hive @skAppKey rk 0))) out "Warning: failed to create backup of defaults."
		rk.Close
	 apply
	c.FromQmGrid(hGrid)
	for i 0 18
		rset "" F"{i+1}" skAppKey hive -2 ;;delete key
		s=c.Cell(i 1); if(!s.len) continue
		sk=F"{skAppKey}\{i+1}"
		if(s~"Disable") s="ShellExecute"; sv="<disable>"
		else
			sv=c.Cell(i 2); if(!sv.len) continue
			sel s 1
				case "Run" s="ShellExecute"
				case ["Association","RegisteredApp"]
				case else continue
		rset sv s sk hive
	 refresh grid
	sub.Work 0 hDlg
	
	case 2 ;;restore
	s=iif(admin "all users" "current user")
	if('O'!mes(F"Restores default key actions for {s}." "" "OCi")) ret
	
	if(!sub_to.RegKeyExists(skAppKeyBackup hive)) ret
	sk.getpath(skAppKey "")
	rset "" "AppKey" sk hive -2 ;;delete key
	i=RegCreateKeyExW(hive @skAppKey 0 0 0 KEY_ALL_ACCESS 0 &rk 0) or SHCopyKeyW(hive @skAppKeyBackup rk 0)
	rk.Close
	if(i) mes "Failed" "" "!"
	else rset "" "AppKey QM Backup" sk hive -2 ;;delete key
	 refresh grid
	sub.Work 0 hDlg
