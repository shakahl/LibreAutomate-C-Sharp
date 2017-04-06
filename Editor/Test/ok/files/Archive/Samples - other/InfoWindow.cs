 \Dialog_Editor
function# [x] [y] [flags] [$title] ;;flags: 1 in this thread, 64 raw x y

 Shows inactive topmost window where macros can display text.
 Returns static text control handle. To display text, use setwintext.
 Does not wait until the dialog is closed.

 x, y - dialog position. Same as with ShowDialog.
 flags:
   1 - create the window in this thread. Can be used if this thread has a dialog or somehow processes Windows messages. Without this flag the window is in other thread.
   64 - same as with ShowDialog.
 title - window title bar text.

 EXAMPLE
 int+ g_myinfowindow
 if(!IsWindow(g_myinfowindow)) g_myinfowindow=InfoWindow(-1 1 0 "QM - InfoWindow")
  ...
 _s="1"; _s.setwintext(g_myinfowindow)
 2
 _s="2"; _s.setwintext(g_myinfowindow)
 2
 clo GetParent(g_myinfowindow)


int h
if flags&0x10001
	sub.Thread x y flags title h
else
	mac "sub.Thread" "" x y flags|0x10000 title &h
	wait 0 V h
ret h


#sub Thread
function x y flags $title &hwnd

str dd=
F
 BEGIN DIALOG
 0 "" 0x80C80848 0x188 0 0 223 136 "{title}"
 3 Static 0x54000000 0x0 4 4 218 130 ""
 END DIALOG

int h=ShowDialog(dd 0 0 0 flags&64|1 0 0 0 x y)
hid- h
hwnd=id(3 h)
if(flags&0x10000=0) ret
opt waitmsg 1
wait 0 -WC hwnd; err
