
 http://www.quickmacros.com/forum/viewtopic.php?f=4&t=1158&p=4337&hilit=listbox+multicolumn#p4337
 dialog_find_text_in_qm_files_Ori

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 383 136 "Dialog"
 3 SysListView32 0x5403804D 0x204 5 4 359 108 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

int hsys32=id(3 hDlg)

 OutWinMsg message wParam lParam

sel message
	case WM_INITDIALOG
	__ImageList-- t_il.Load("$qm$\il_qm.bmp")
	SendMessage hsys32 LVM_SETIMAGELIST LVSIL_SMALL t_il
	
	int es=LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP|LVS_EX_SUBITEMIMAGES
	SendMessage hsys32 LVM_SETEXTENDEDLISTVIEWSTYLE es es
	sub.LvAddCol hsys32 0 "Label" -80
	sub.LvAddCol hsys32 1 "QM-Item" -20

 	Fill-in values to sys32
	SendMessage hsys32 LVM_DELETEALLITEMS 0 0
	str col0=
	 erqwer qwer
	 qwerqwer 
	 sdf gfsfdgsdf
	 11231231 gdfdfg
	 mnvbntghy rtyed
	str col1=
	 1
	 2
	 3
	 4
	 5
	str s0 s1
	int n=numlines(col0)
	for int'i 0 n
		s0.getl(col0 i)
		s1.getl(col1 i)
		sub.LvAdd hsys32 i 0 i s0 s1
		sub.LvSetCellImage hsys32 i 1 i+5
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3

ret
 messages2
 OutWinMsg message wParam lParam

sel wParam
	case IDOK
	case IDCANCEL
ret 1

 messages3
NMHDR* nh=+lParam
NMITEMACTIVATE* na=+nh
int j
sel nh.code
	case LVN_ITEMCHANGED
	if(na.uNewState&LVIS_SELECTED and na.uOldState&LVIS_SELECTED=0) ;;listview item selected
		sel nh.idFrom
			case 3 
			j=na.iItem;; if(j<0 or j>=aFiles.len) ret
			out j
			
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


#sub LvSetCellImage
function hlv row column imageIndex

LVITEMW lvi.mask=LVIF_IMAGE
lvi.iItem=row
lvi.iSubItem=column
lvi.iImage=imageIndex
SendMessage(hlv LVM_SETITEMW 0 &lvi)
