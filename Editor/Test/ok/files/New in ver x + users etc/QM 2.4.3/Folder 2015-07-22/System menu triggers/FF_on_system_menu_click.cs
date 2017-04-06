 /
function# iid FILTER&f

if(GetClassLong(f.hwnd GCW_ATOM)!=32768) ret -2 ;;is popup menu?
int hwnd=GetWindow(f.hwnd GW_OWNER)
if(!hwnd) ret -2 ;;context menu; else system or window menu
int hmenu=GetProp(hwnd "symt_hmenu"); if(!hmenu) ret -2 ;;has QM-added submenus?
if(hmenu!=SendMessage(f.hwnd MN_GETHMENU 0 0)) ret -2 ;;is system menu?
 int hmenu=GetSystemMenu(hwnd 0)
POINT p; xm p
int itemIndex=MenuItemFromPoint(hwnd hmenu p) ;;note: will not work with DPI-scaled windows
int itemId=GetMenuItemID(hmenu itemIndex)
if(!(itemId>=0xA000 and itemId<=0xAFFF)) ret -2 ;;assume we always add menu items with id in this range

str s1.getwinclass(hwnd) s2.getwintext(hwnd) s3.format("%s[]%s" s1 s2)
sel s3 3
	case "Notepad[]*"
	out "itemIndex=%i itemId=%i" itemIndex itemId

ret -1
