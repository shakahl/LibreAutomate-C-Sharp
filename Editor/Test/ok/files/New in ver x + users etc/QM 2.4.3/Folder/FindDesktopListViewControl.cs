 /
function#

 Returns the handle of the SysListView32 control that displays desktop icons.


int w=GetShellWindow
int c=child("" "SysListView32" w 0 "id=1")
if !c
	w=win("" "WorkerW" "" 0 F"threadId={GetWindowThreadProcessId(w 0)}"); if(!w) goto gErr
	c=child("" "SysListView32" w 0 "id=1"); if(!c) goto gErr

ret c
 gErr
end "desktop window not found"
