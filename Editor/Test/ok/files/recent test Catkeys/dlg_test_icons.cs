
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 564 294 "Dialog"
 3 QM_Grid 0x56031041 0x200 0 0 564 294 "0x4,0,0,0x0,0x0[]A,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

 ARRAY(__Hicon) a

str controls = "3"
str qmg3x
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

DlgGrid g.Init(id(3 hDlg))
sel message
	case WM_INITDIALOG
	goto gInit
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 gInit

ARRAY(str) a
 GetFilesInFolder a "q:\app" "" 2
GetFilesInFolder a "$system$" "*.exe"
 GetFilesInFolder a "$system$" "*.dll"
 GetFilesInFolder a "q:\app" "(?i)\.(ico|exe)$" 2|0x10000
 GetFilesInFolder a "q:\app" "(?i)\.(ico)$" 2|0x10000
 GetFilesInFolder a "q:\app" "(?i)\.(exe)$" 2|0x10000
int i

ARRAY(__Hicon) ai.create(a.len)
PF
for i 0 a.len
	ai[i]=sub.Icon2(a[i] 0)
PN;PO

__ImageList- il.Create()
 __ImageList- il.Create("" 32)
for i 0 a.len
	int hi=ai[i]
	if(hi=0) hi=GetFileIcon("" 0 8)
	ImageList_ReplaceIcon(il -1 hi)
	g.CellSet(i 0 F"<//{i}>{a[i]}")

g.SetImagelist(il)


#sub Icon
function# $sf flags ;;flags: 1 large

 ret GetFileIcon(sf 0 flags|8)

int hr fl
fl=GIL_SIMULATEDOC
if(flags&1) hr=SHDefExtractIconW(@sf 0 fl &_i 0 0)
else hr=SHDefExtractIconW(@sf 0 fl 0 &_i 0)
 if(hr) out hr; ret
ret _i


#sub Icon2
function# $sf flags ;;flags: 1 large

dll shell32 #SHExtractIconsW @*pszIconFile iIndex cx cy *phicon *pRI n flags

int hi ri cx cy
if(flags&1) cx=32; cy=32; else cx=16; cy=16
if(!SHExtractIconsW(@sf 0 cx cy &hi &ri 1 0)) ret
ret hi


#sub Icon3
function# $sf flags ;;flags: 1 large

int hi ri cx cy
if(flags&1) cx=32; cy=32; else cx=16; cy=16
if(!PrivateExtractIconsW(@sf 0 cx cy &hi &ri 1 0)) ret
ret hi
