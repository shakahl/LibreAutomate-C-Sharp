 /
function# str&s [$initDir] [flags] [$text] [hwndOwner] ;;flags: 1 include files, 2 include non-file objects (Control Panel, etc), 4 new style

 Shows "Browse For Folder" dialog for selecting a folder.
 Returns a nonzero value on OK, 0 on Cancel.

 s - variable that receives full path of selected folder.
   With flag 2 it is <help #IDP_SEARCHPATHS>ITEMIDLIST string</help> if a non-file object selected.
 initDir - initially display this folder in the dialog.
   Without flag 4 displays only this folder and subfolders. With flag 4 displays all folders and selects/expands this folder.
 flags:
   1, 2, 4 - see above.
   Flag 4 changes dialog style, adds more features.
   Also can include BIF_x flags (see <google "site:microsoft.com BROWSEINFO">BROWSEINFO</google>), left-shifted by 8 bits. For example, to browse for computer, use 2|(WINAPI.BIF_BROWSEFORCOMPUTER<<8).
 text - text to display above the folder list.
 hwndOwner (QM 2.3.4) - owner window handle. It will be disabled.
   If omitted or 0, will use the active window, if it belongs to current thread. To create unowned dialog, use GetDesktopWindow for hwndOwner.

 EXAMPLE
 str s
 if(!BrowseForFolder(s "$windows$" 4)) ret
 out s


BROWSEINFOW b
if(!hwndOwner) hwndOwner=win; if(GetWindowThreadProcessId(hwndOwner 0)!=GetCurrentThreadId) hwndOwner=0
b.hwndOwner=hwndOwner
if(flags&1) b.ulFlags|BIF_BROWSEINCLUDEFILES
if(flags&2=0) b.ulFlags|BIF_RETURNONLYFSDIRS
if(flags&4) b.ulFlags|BIF_USENEWUI
b.ulFlags|flags>>8
b.lpszTitle=@text
if(!empty(initDir)) ITEMIDLIST* initPidl=PidlFromStr(initDir)
if(flags&4) b.lpfn=&sub.Callback
else b.pidlRoot=initPidl ;;BFFM_SETEXPANDED does not work with old-style

ITEMIDLIST* pidl=SHBrowseForFolderW(&b)

if(pidl) PidlToStr(pidl &s flags&2=0); CoTaskMemFree pidl; else s.all
CoTaskMemFree initPidl
0
ret s.len


#sub Callback v
function# hwnd uMsg lParam !*lpData

sel uMsg
	case BFFM_INITIALIZED
	if(initPidl) SendMessage hwnd BFFM_SETEXPANDED 0 initPidl
	case BFFM_SELCHANGED
	if(flags&6=4 and _winnt<6) SendMessage hwnd BFFM_ENABLEOK 0 PidlToStr(+lParam &_s 1) ;;xp bug fix
