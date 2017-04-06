\Dialog_Editor

str dd=
 BEGIN DIALOG
 1 "" 0x90CE0AC8 0x0 0 0 424 134 "Find text in QM files"
 3 Edit 0x54231044 0x200 0 0 216 20 ""
 10 Button 0x54012003 0x0 224 4 54 10 "Match case"
 11 Button 0x54012003 0x0 284 4 58 10 "Whole word"
 12 Button 0x54012003 0x0 348 4 50 10 "Regexp"
 6 Button 0x54032000 0x4 8 24 40 24 "Find"
 7 Button 0x4C032000 0x4 56 28 40 14 "Stop"
 4 Static 0x54000200 0x4 116 28 32 13 "In folder"
 5 ComboBox 0x54230242 0x4 152 28 208 213 ""
 13 Button 0x54012003 0x0 364 28 58 13 "+ subfolders"
 8 SysListView32 0x5403804D 0x204 8 52 240 77 ""
 9 SysListView32 0x5403800D 0x204 256 52 160 77 ""
 14 Button 0x54032000 0x0 400 4 16 13 "RX"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

int hDlg hFiles hItems
ARRAY(str) aFiles aItems
str currentFile

str controls = "3 10 11 12 5 13"
str e3 c10Mat c11Who c12Reg cb5 c13
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret


#sub DlgProc v
function# hdlg message wParam lParam

sel message
	case WM_INITDIALOG sub.Init hdlg
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 6 sub.Find ;;Find
	case 7 ;;Stop
	case 14 RegExpMenu id(3 hDlg) ;;RX
	
ret 1
 messages3
NMHDR* nh=+lParam
NMITEMACTIVATE* na=+nh
int i
sel nh.code
	case LVN_ITEMCHANGED
	if(na.uNewState&LVIS_SELECTED and na.uOldState&LVIS_SELECTED=0) ;;listview item selected
		sel nh.idFrom
			case 8 ;;files
			SendMessage hItems LVM_DELETEALLITEMS 0 0
			i=na.iItem; if(i<0 or i>=aFiles.len) ret
			hid hItems ;;10 times faster; we don't use ownerdraw
			currentFile=aFiles[i]
			sub.FindInFile(currentFile 1)
			hid- hItems
			
			case 9 ;;items
			sub.OpenItem na.lParam


#sub Init v
function hdlg

hDlg=hdlg; hFiles=id(8 hDlg); hItems=id(9 hDlg)

int h=id(5 hDlg)
rget _s "backup folder" "software\gindi\qm2\settings" 0 "$my qm$\backup"; CB_Add h _s
rget _s "file" "software\gindi\qm2\settings" 0 "$my qm$\main.qml"; CB_Add h _s.getpath(_s "")
CB_SelectItem h 0

int es=LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP
SendMessage hFiles LVM_SETEXTENDEDLISTVIEWSTYLE es es
SendMessage hItems LVM_SETEXTENDEDLISTVIEWSTYLE es es
sub.LvAddCol hFiles 0 "File" -54
sub.LvAddCol hFiles 1 "Date modified" -40
sub.LvAddCol hItems 0 "Item" -90
SendMessage hItems LVM_SETIMAGELIST LVSIL_SMALL __ImageListFromIcons("$qm$\empty.ico[]$qm$\macro.ico[]$qm$\function.ico[]$qm$\menu.ico[]$qm$\toolbar.ico[]$qm$\tsm.ico[]$qm$\folder.ico[]$qm$\member.ico[]$qm$\filelink.ico" 16)

DT_SetAutoSizeControls hDlg "8sv 9s"


#sub Find v

str sFolder
int i j

SendMessage hFiles LVM_DELETEALLITEMS 0 0
SendMessage hItems LVM_DELETEALLITEMS 0 0
aFiles=0; aItems=0
sFolder.getwintext(id(5 hDlg))
ARRAY(str) a
i=32; if(but(13 hDlg)) i|4
GetFilesInFolder a sFolder "*.qml" i 2
if(!a.len) ret
a.sort(9)
for i 0 a.len
	DateTime t=val(a[i] 1 _i)
	str sPath=a[i]+_i+1
	j=sub.FindInFile(sPath 0); if(!j) continue; else if(j<0) break
	t.UtcToLocal
	aFiles[]=sPath
	sub.LvAdd hFiles i 0 -2 sPath.getfilename t.ToStr(4)


#sub FindInFile v
function# $sPath !all ;;returns: 1 found, 0 not found, -1 rx error

str sFind.getwintext(id(3 hDlg)); if(!sFind.len) ret 1
int flags=!but(10 hDlg)|(but(11 hDlg)*2)|(but(12 hDlg)*4) ;;1 insens, 2 word, 4 regexp
str sf
if(flags&4) findrx("" sFind 0 flags&3|128 sf); err out _error.description; ret -1
else sf=sFind
int found i
Sqlite x.Open(sPath 1)
SqliteStatement t g
t.Prepare(x "SELECT text,rowid FROM texts")
g.Prepare(x "SELECT name,flags FROM items WHERE rowid=?1")
rep
	if(!t.FetchRow) break
	lpstr st=t.GetText(0)
	 find sf in st
	if(flags&4) i=findrx(st sf)
	else if(flags&2) i=findw(st sf 0 "" flags&1|64)
	else i=find(st sf 0 flags&1)
	if(i<0) continue
	found=1
	if(!all) break
	 add to items listview
	str s; int rowid=t.GetInt(1)
	g.BindInt(1 rowid)
	g.FetchRow
	s=g.GetText(0)
	sub.LvAdd hItems -1 rowid g.GetInt(1)&255 s
	g.Reset
ret found
err+ out F"Cannot search in {sPath}. Error: {_error.description}"


#sub OpenItem v
function rowid
Sqlite x.Open(currentFile 1)
ARRAY(str) a
x.Exec(F"SELECT text FROM texts WHERE rowid={rowid}" a)
 QmHelp F"<code>{a[0 0]}</code>" 0 6
newitem "Find text in QM files" a[0 0] "Macro" "" "" 3|4|128


#sub LvAddCol
function hlv index $txt width

 Adds column to SysListView32 control that has LVS_REPORT style (1).
 Index of first colunm is 0. If index <0, adds to the end.
 If width <0, it is interpreted as -percentage.


LVCOLUMNW col.mask=LVCF_WIDTH|LVCF_TEXT
col.pszText=@txt
if(width<0) RECT r; GetClientRect hlv &r; width=-width*r.right/100
col.cx=width
if(index<0) index=0x7fffffff
SendMessage hlv LVM_INSERTCOLUMNW index &col


#sub LvAdd
function# hlv index lparam image ~s [~s1] [~s2] [~s3] [~s4] [~s5] [~s6] [~s7] [~s8] [~s9]

 Adds item to SysListView32 control that has LVS_REPORT style and 1 to 10 columns.


if(index<0) index=0x7FFFFFFF
LVITEMW lvi.mask=LVIF_TEXT|LVIF_PARAM|LVIF_IMAGE
lvi.iItem=index
lvi.pszText=@s
lvi.lParam=lparam
lvi.iImage=image
index=SendMessage(hlv LVM_INSERTITEMW 0 &lvi)

if index>=0
	int i; str* p=&s
	for i 1 getopt(nargs)-4
		lvi.iItem=index
		lvi.iSubItem=i
		lvi.pszText=@p[i]
		SendMessage(hlv LVM_SETITEMTEXTW index &lvi)

ret index
