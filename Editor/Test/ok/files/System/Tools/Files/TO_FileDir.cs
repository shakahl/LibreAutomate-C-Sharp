 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 5 6 11 13 15 16 43"
__strt e4Fol lb5Typ lb6Whe lb11Get lb13Att cb15Tim c16Tim c43No

lb5Typ="&Only files[]Only folders[]Files and folders"
lb6Whe="&Only in this folder[]Only in subfolders[]In this folder and subfolders"
lb11Get="&Full path[]Filename[]Attributes[]Size[]Time modified[]Time created[]Time accessed[]Is folder[]Data"
lb13Att=__S_FILE_ATTR
cb15Tim="&UTC time[]Local time"

if(!ShowDialog("" &TO_FileDir &controls _hwndqm)) ret

 gtest

__strt& g=lb11Get
str s
__strt vd v vAttr
int f test

if(test) s="Dir d"; v="d"; else s=F"{vd.VD(`Dir d` v)}"

if(!e4Fol.len) e4Fol="''*''"
else e4Fol.S; if(e4Fol[0]=34 and findcs(e4Fol "*?.")<0) e4Fol.rtrim("''\"); e4Fol.s+"\*''"

f|val(lb5Typ)
sel(val(lb6Whe)) case 1 f|12; case 2 f|4

s+F"[]foreach({v} {e4Fol} FE_Dir {vd.Flags(f 2)}"
sub_to.Trim s " 0"; s+")[][9]"

if test
	s+"out d.FullPath[]"
	ret sub.Test(hDlg s)

str sf=
 str path=#.FullPath
 str name=#.FileName
 int attr=#.FileAttributes
 long size=#.FileSize
 DateTime tm=#.TimeModifiedUTC
 DateTime tc=#.TimeCreatedUTC
 DateTime ta=#.TimeAccessedUTC
 int isFolder=#.IsFolder
 str data.getfile(#.FullPath);; err ...
sf.findreplace("#" v)
if(val(cb15Tim)=1) sf.findreplace("UTC" "Local")

int i
ARRAY(str) a aa
if(lb13Att.LbSelectedItemsToNames(aa __S_FILE_ATTR "" 1)) g[2]='1'
g.LbSelectedItemsToNames(a sf "100000000" 1)

for i 0 a.len
	str& r=a[i]; if(!r.len) continue
	s+F"{vd.VD(r v.all)}[][9]"
	sel i
		case 0 s+F"out {v}[][9]"
		case 2 vAttr=v
		case [4,5,6] if(c16Tim=1) s+F"_s={v}.ToStr(4); out _s[][9]"

for i 0 aa.len
	&r=aa[i]; if(!r.len) continue
	s+F"if({vAttr}&FILE_ATTRIBUTE_{r}) out ''{r}''[][9]"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 299 210 "Enumerate files"
 3 Static 0x54020000 0x4 4 6 126 12 "Folder\pattern, like C:\doc\*.txt"
 4 Edit 0x54030080 0x204 4 20 292 14 "Fol"
 5 ListBox 0x54230101 0x200 4 44 102 32 "Type"
 6 ListBox 0x54230101 0x200 126 44 102 32 "Where"
 10 Static 0x54000000 0x0 4 86 60 10 "Get"
 11 ListBox 0x54230109 0x200 4 98 102 78 "Get" "Select one or several"
 12 Static 0x54000000 0x0 126 86 60 10 "Test attributes"
 13 ListBox 0x54230109 0x200 126 98 102 78 "Att" "Select one or several"
 15 ComboBox 0x54230243 0x0 244 146 52 213 "Tim"
 16 Button 0x54012003 0x0 244 162 58 12 "Time string"
 7 Button 0x54032000 0x4 160 4 50 14 "Browse..."
 8 Button 0x54032000 0x4 212 4 16 14 "SF" "Special folders"
 43 Button 0x54012003 0x0 240 4 40 13 "No SF" "Let the button give me normal path, not special folder name"
 1 Button 0x54030001 0x4 4 190 48 14 "OK"
 2 Button 0x54030000 0x4 54 190 48 14 "Cancel"
 41 Button 0x54032000 0x4 104 190 16 14 "?"
 14 Button 0x54032000 0x0 122 190 34 14 "? More"
 9 Button 0x54032000 0x0 158 190 34 14 "Test" "Test whether code created by this dialog will work as you want, and how fast.[]If works, shows all matching files."
 44 Button 0x54032000 0x0 194 190 34 14 "Stop" "Stop testing"
 45 Static 0x54000000 0x0 236 192 60 12 ""
 46 Static 0x54000010 0x20000 0 182 322 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040101 "*" "" "" ""

ret
 messages
int- t_testThread
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\files.ico" "4" 1)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7 sub_to.FolderDialog hDlg 4
	case 8 sub_to.File_SF hDlg 4 "\"
	case 41 QmHelp "Dir Help"
	case 14 QmHelp "GetFilesInFolder"
	case 9
	DT_GetControls(hDlg &controls)
	test=1; goto gtest
	case 44 if(t_testThread) EndThread "" t_testThread 2; t_testThread=0
ret 1

#opt nowarnings 1


#sub Test
function hDlg str&s

int- t_testThread
if(t_testThread) EndThread "" t_testThread 2; t_testThread=0
int ht=sub_to.Test(hDlg s 0 1 45)
QMTHREAD t; GetQmThreadInfo ht &t; t_testThread=t.tuid


#ret
function hDlg
out "[]---- test ----[]"
#ret
