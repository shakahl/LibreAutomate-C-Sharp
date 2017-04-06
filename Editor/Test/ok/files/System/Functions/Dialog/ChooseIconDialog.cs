 /
function# str&path int&index [$defaultPath] [defaultIndex] [hwndOwner]

 Shows "Change Icon" dialog.
 Returns 1 on OK, 0 on Cancel.

 path - str variable that receives icon file path.
 index - int variable that receives icon index.
 defaultPath, defaultIndex - can be used to initialize the dialog.
 hwndOwner - handle of owner window or 0.

 EXAMPLE
 str s; int i
 if(!ChooseIconDialog(s i)) ret
 out "%s,%i" s i


str sd.expandpath(defaultPath); if(sd.len>MAX_PATH) sd.all
index=defaultIndex

BSTR b.alloc(MAX_PATH)
if(sd.len) lstrcpynW b.pstr @sd MAX_PATH

if(!PickIconDlg(hwndOwner b MAX_PATH+1 &index)) 0; ret
path.ansi(b)
path.expandpath ;;can contain %envvar%
0
ret 1
