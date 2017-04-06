 \FtpDirDlg
function htv

type FTPDIRDATA Ftp'ftp htv ifoldericon
FTPDIRDATA f

f.ftp.Connect("ftp.arm.linux.org.uk" "anonymous" "anonymous")
f.htv=htv

 use system image list; get folder icon index
SHFILEINFO fi
int il=SHGetFileInfo(0 FILE_ATTRIBUTE_DIRECTORY &fi sizeof(fi) SHGFI_SYSICONINDEX|SHGFI_USEFILEATTRIBUTES|SHGFI_SMALLICON)
if _winnt<6
	f.ifoldericon=fi.iIcon ;;incorrect on Win 7
else
	__Hicon hi=GetFileIcon("$qm$")
	f.ifoldericon=ImageList_ReplaceIcon(il -1 hi)

SendMessage(htv TVM_SETIMAGELIST 0 il)

FD_Folder f 0 "/pub/linux/IPX"
