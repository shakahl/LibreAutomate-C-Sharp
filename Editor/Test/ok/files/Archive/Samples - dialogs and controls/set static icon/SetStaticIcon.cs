 /
function hwndControl [$iconFile] [flags] ;;flags: 1 large

 Sets/changes/deletes icon of a static icon control.

 hwndControl - control handle.
 iconFile - icon file. If "" or not used, removes icon.
    To avoid memory leak, always call this function on WM_DESTROY with empty iconFile.

 Initially the control must not have icon.


int hi
if(!empty(iconFile)) hi=GetFileIcon(iconFile 0 flags)
hi=SendMessage(hwndControl STM_SETICON hi 0)
if(hi) DestroyIcon hi
