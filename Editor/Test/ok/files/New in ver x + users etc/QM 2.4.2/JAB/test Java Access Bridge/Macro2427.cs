out
#compile "__JavaAO"
SetEnvVar "jab_dll" "WindowsAccessBridge"
type JOBJECT64 :int
qmjab_api

dll- user32 #ChangeWindowMessageFilter message dwFlag
if _winnt>=6
	ChangeWindowMessageFilter(RegisterWindowMessage("AccessBridge-FromJava-Hello") 1) ;;received by the hidden "Access Bridge status" dialog once after initializeAccessBridge
	ChangeWindowMessageFilter(RegisterWindowMessage("AccessBridge-FromWindows-Hello") 1) ;;received by the dialog for each JAB-enabled Java window. initializeAccessBridge posts this message to all windows. wParam is HWND of the poster dialog.

JAB.initializeAccessBridge
 0
opt waitmsg 1; 0.1
 JAB.SetFocusGained &sub.OnFocusGained
 mes 1; ret

 int w=win("Global Options jEdit: General" "SunAwtDialog")
 int w=win("" "SunAwtFrame")
 int w=win("" "SunAwtDialog")
 act w
 lef 1156 354 ;; 'jEdit - Untitled-1'
 lef 240 300 w 1 ;; 'Global Options jEdit: General'
 0.1
int w=win(mouse); lef

JavaAO x xx; Acc a
if 1
	if(!JAB.GetAccessibleContextWithFocus(w &x.vmID &x.a)) ret
	a.a=JabAccFromAC(x.vmID xx.a)
	out x.a
	a.a=JabAccFromAC(x.vmID x.a)
else
	if(!JAB.GetAccessibleContextFromHWND(w &x.vmID &x.a)) ret
	if(!JAB.GetAccessibleContextAt(x.vmID x.a xm ym &xx.a)) ret
	xx.vmID=x.vmID
	out x.a
	 out xx.a
	 out JAB.getActiveDescendent(xx.vmID xx.a)
	 out x.vmID
	a.a=JabAccFromAC(x.vmID xx.a)
out a.a
a.showRECT
 out a.Name


#sub OnFocusGained
function[c] vmID JOBJECT64'event JOBJECT64'source
out "OnFocusGained: %i %i" event source
