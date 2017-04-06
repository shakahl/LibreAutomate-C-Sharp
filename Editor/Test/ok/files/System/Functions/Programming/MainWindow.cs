function# [$name] [$classname] [wndproc] [x] [y] [width] [height] [style] [hwndParent] [!*pdata] [exstyle] [$classIcon] [$classIconSmall] [classWndExtra] [classStyle]

 Helps to create a simple application with a window.
 Note: You can instead use a dialog. It's simpler.

 1. Registers window class (calls <help>RegWinClass</help>).
 2. Creates and shows main window (calls <google>CreateWindowEx</google>).
 3. Gets messages in message loop (calls <help>MessageLoop</help>).

 Returns when the window procedure calls PostQuitMessage. Usually it is called on WM_DESTROY message.

 name - window name.
 classname - window class. Must be some unique string.
   QM 2.3.0. If classname begins with "@", creates Unicode class and window. Unicode window procedure must call DefWindowProcW, not DefWindowProc.
 wndproc - address of <help "WndProc_Normal">window procedure</help>.
   A template is available in menu -> File -> New -> Templates.
 Next 8 parameters are the same as with <google>CreateWindowEx</google>.
   If style is 0, uses WS_OVERLAPPEDWINDOW|WS_VISIBLE|WS_CLIPCHILDREN.
   Default width and height are 600 400.
 Last 4 parameters are used with <google>WNDCLASSEX</google>. Added in QM 2.3.0.

 REMARKS
 A typical Windows application registers main window's class, creates main window and runs message loop, which runs until the main window closed. This function does all it.
 Other required function is window procedure. It is called whenever Windows, the application or other applications send or post a message to a window that belongs to that class. It processes some messages, and/or calls DefWindowProc, which does default processing.
 This function internally calls <help>MessageLoop</help>. You can use <help>MessageLoopOptions</help> to set keyboard navigation etc.


int h unic; str cl

if(empty(classname)) cl="QM_Window_Class"; else if(classname[0]='@') cl=classname+1; unic=4; else cl=classname

if(!wndproc) wndproc=&WndProc_Normal

__Hicon hicon hiconSm
if(!empty(classIcon)) hicon=GetFileIcon(classIcon 0 1); else hicon=__GetQmItemIcon(+getopt(itemid 3) 1)
if(!empty(classIconSmall)) hiconSm=GetFileIcon(classIconSmall); else hiconSm=__GetQmItemIcon(+getopt(itemid 3) 0)

RegWinClass cl wndproc unic hicon classWndExtra classStyle hiconSm

if(style=0) style=WS_OVERLAPPEDWINDOW|WS_VISIBLE|WS_CLIPCHILDREN; else style~WS_CHILD
if(width<1) width=600; if(height<1) height=400

h=CreateWindowExW(exstyle @cl @name style x y width height hwndParent 0 _hinst pdata)
if(!h) ret ;;no warning etc, because wndproc eg can return -1 on wm_create to destroy the window

if(hwndParent) MoveWindowToMonitor h hwndParent 1

ret MessageLoop
