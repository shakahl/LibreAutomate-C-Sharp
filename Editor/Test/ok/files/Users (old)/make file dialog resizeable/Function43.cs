 int hwnd=val(_command)
 out hwnd
RECT Rect
RECT LCRect
def ANCHORE_LEFT   0x0000
def ANCHORE_TOP      0x0000
def ANCHORE_RIGHT   0x0001
def ANCHORE_BOTTOM  0x0002
def RESIZE_HOR      0x0004
def RESIZE_VER      0x0008
def RESIZE_BOTH     (RESIZE_HOR | RESIZE_VER)
int hwnd=val(_command)
int hlv=child("FolderView" "SysListView32" hwnd 0x1)
int st = GetWinStyle(hwnd)
if(st=WS_THICKFRAME) goto something

SetWinStyle hwnd WS_THICKFRAME 1
int sys_menu=GetSystemMenu(hwnd 0)
AppendMenu(sys_menu MF_BYPOSITION|MF_STRING SC_SIZE "&Size")
int x y cx cy; GetWinXY hlv x y cx cy

end
 something
out "huhu"
