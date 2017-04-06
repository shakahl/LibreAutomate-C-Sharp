 /test SelectQmItemDialogWithTreeview
 \Dialog_Editor
function! str&sSelectedItem [$initFolder] [hwndOwner]

 Shows a dialog with a treeview control that displays a QM folder.
 The user can expand subfolders and select an item or folder.
 On OK stores full path of the selected QM item or folder in sSelectedItem and returns 1. On Cancel returns 0.

 EXAMPLE
 str s
 if SelectQmItemDialogWithTreeview(s "\System\Functions")
	 out s


ARRAY(str) a ;;full paths of all displayed items; treeview lparam is its element index
 __ImageList il=ImageList_Create(16  ;;maybe later

str dd=
 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 224 280 "SelectQmItemDialogWithTreeview"
 3 SysTreeView32 0x54030823 0x0 0 0 224 256 ""
 1 Button 0x54030001 0x4 116 260 48 14 "OK"
 2 Button 0x54030000 0x4 168 260 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 hwndOwner)) ret
ret 1


#sub DlgProc v
function# hDlg message wParam lParam

int i htv htvi
sel message
	case WM_INITDIALOG
	 add initFolder to the treeview and expand; we'll catch TVN_ITEMEXPANDINGW and add children
	htv=id(3 hDlg)
	htvi=sub.TvAdd(htv 0 initFolder -1 1)
	SendMessage htv TVM_EXPAND TVE_EXPAND htvi
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 sSelectedItem = full path of the selected file
	htv=id(3 hDlg)
	htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_CARET 0)
	i=TvGetParam(htv htvi); if(i<0 or i>=a.len) ret
	sSelectedItem=a[i]
	
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
NMTREEVIEWW* nt=+nh
if(nh.idFrom!=3) ret
sel nh.code
	case TVN_ITEMEXPANDINGW
	 enum this folder and add children to the treeview, unless already done for this folder
	if(nt.action&TVE_EXPAND=0) ret
	i=nt.itemNew.lParam; if(i<0 or i>=a.len) ret
	if nt.itemNew.cChildren=1 and !SendMessage(nh.hwndFrom TVM_GETNEXTITEM TVGN_CHILD nt.itemNew.hItem)
		sub.AddFolderChildren nh.hwndFrom nt.itemNew.hItem a[i]
	
	case TVN_SELCHANGEDW
	i=nt.itemNew.lParam; if(i<0 or i>=a.len) ret
	 out a[i]
	out _s.getmacro(a[i]) ;;see also qmitem, mac+


#sub AddFolderChildren v
function htv htviFolder $folder

 Enumerates direct child items and subfolders of folder, and adds to the treeview.
 We'll add their children later on demand, on TVN_ITEMEXPANDINGW.

if(!folder) folder=""
ARRAY(QMITEMIDLEVEL) af
GetQmItemsInFolder(folder &af 1)

if(!empty(folder) and folder[0]!'\') str _s1; folder=_s1.from("\" folder)
int i
for i 0 af.len
	QMITEM q; qmitem(af[i].id 0 q 1)
	sub.TvAdd(htv htviFolder F"{folder}\{q.name}" -1 q.itype=5)


#sub TvAdd v
function# htv htviFolder $fullPath iimage cChildren

 Adds treeview item. Also adds fullPath to a.

TVINSERTSTRUCTW in
in.hParent=htviFolder
in.hInsertAfter=TVI_LAST
TVITEMW& r=in.item
r.mask=TVIF_TEXT|TVIF_PARAM
_s.getfilename(fullPath 1); if(!_s.len) _s=fullPath
r.pszText=@_s
if iimage>=0
	r.mask|=TVIF_IMAGE|TVIF_SELECTEDIMAGE
	r.iImage=iimage
	r.iSelectedImage=iimage
if(cChildren) r.cChildren=cChildren; r.mask|=TVIF_CHILDREN
r.lParam=a.len; a[]=fullPath

ret SendMessage(htv TVM_INSERTITEMW 0 &in)
