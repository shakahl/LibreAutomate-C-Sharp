 /
function ARRAY(int)&a [flags] ;;flags: 4 all desktops

 Gets handles of windows that would be added to taskbar.

 a - variable for handles.
 flags (QM 2.4.3):
   4 - on Windows 10, include windows in all virtual desktops, not only in the current desktop. On Windows 8, include Windows store apps.

 REMARKS
 If you get handles of all windows with <help>win</help> (with array), you see that there are many windows that are not in taskbar. These are hidden, owned, toolwindows, etc. This function filters out all these windows. However it is not always possible to know what windows are in taskbar, therefore sometimes this function may also get some windows that are not there. The order of window handles in array a will match the Z order, not the order of taskbar buttons. This function uses <help>RealGetNextWindow</help>.

 Added in: QM 2.3.4.


a=0
rep
	int w=RealGetNextWindow(w flags&4)
	if(!w) break
	a[]=w
