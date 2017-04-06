 /
function hwnd $view ;;view: "d" details, "h" thumbnails, "l" list, "n" icons, "s" tiles, "g" large icons (Win2000), "m" small icons (Win2000)

 Changes view in standard file "Open" or "Save" dialog.
 Error if failed.

 hwnd - dialog window handle.
 view - string consisting of the underlined character in the view menu.

 REMARKS
 Works only on Windows XP, 2000, 2003. On other OS does nothing.
 May not work on non-English Windows, because uses toolbar button text.
 Activates the window.

 EXAMPLE
 int w=win("Open" "#32770")
 FileOpenDialogChangeView w "d" ;;set detals view


if(_winnt>=6) ret
if(!hwnd) end "invalid window handle"

spe -1
act hwnd

Acc a=acc("View Menu" "PUSHBUTTON" hwnd "ToolbarWindow32" "" 0x1001)
 a.DoDefaultAction ;;does not work
 a.Mouse(1); mou ;;works, but...
int x y h=child(a)
a.Location(x y); ScreenToClient h +&x
int m=MakeInt(x y)
PostMessage h WM_LBUTTONDOWN 0 m
PostMessage h WM_LBUTTONUP 0 m

int w=wait(5 WV "+#32768")
key (view)

err+ end "failed"

 Could not find a better way. Google - no. ShellWindows - no. AccessibleObjectFromWindow/QueryService - no.
