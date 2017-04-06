out
 find SysListView32 control that displays desktop icons
int w=GetShellWindow
int c=child("FolderView" "SysListView32" w 0 "id=1")
if !c
	w=win("" "WorkerW" "" 0 F"threadId={GetWindowThreadProcessId(w 0)}")
	c=child("FolderView" "SysListView32" w 0 "id=1")
if(!c) end "desktop window not found"

 get selected items
Acc ac.FromWindow(c OBJID_CLIENT)
int i=-1
rep
	i=SendMessage(c LVM_GETNEXTITEM i LVNI_SELECTED)
	if(i<0) break
	ac.elem=i+1
	str txt=ac.Name
	out txt

 However cannot get paths, extensions.
 I did not find a way to get COM interface of desktop like of open folder windows.
