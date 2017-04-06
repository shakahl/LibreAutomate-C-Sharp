 /test SelectFileDialogWithTreeview
 \Dialog_Editor
function! str&sSelectedFile [$initFolder] [hwndOwner]

 Shows a dialog with a treeview control that displays a file folder.
 The user can expand subfolders and select a file or folder.
 On OK stores full path of the selected file or folder in sSelectedFile and returns 1. On Cancel returns 0.
 Similar to OpenSaveDialog.

 EXAMPLE
 str s
 if SelectFileDialogWithTreeview(s "C:")
	 out s


if(empty(initFolder)) initFolder="$documents$"
str _sInitF; _sInitF.expandpath(initFolder); _sInitF.rtrim("\"); initFolder=_sInitF
ARRAY(str) a ;;full paths of all displayed files; treeview lparam is its element index
 __ImageList il=ImageList_Create(16  ;;maybe later

str dd=
 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 224 280 "SelectFileDialogWithTreeview"
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
	 sSelectedFile = full path of the selected file
	htv=id(3 hDlg)
	htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_CARET 0)
	i=TvGetParam(htv htvi); if(i<0 or i>=a.len) ret
	sSelectedFile=a[i]
	
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


#sub AddFolderChildren v
function htv htviFolder $folder

 Enumerates direct child files and subfolders of folder, and adds to the treeview.
 We'll add their children later on demand, on TVN_ITEMEXPANDINGW.

 at first add files and folders to separate arrays, because we want to display folders first
ARRAY(str) aFolders aFiles
int lastFolder=TVI_FIRST
Dir d
foreach(d F"{folder}\*" FE_Dir 2|64)
	str path=d.FullPath
	if(d.IsFolder) aFolders[]=path; else aFiles[]=path

int i
for(i 0 aFolders.len) sub.TvAdd(htv htviFolder aFolders[i] -1 1)
for(i 0 aFiles.len) sub.TvAdd(htv htviFolder aFiles[i] -1 0)


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
