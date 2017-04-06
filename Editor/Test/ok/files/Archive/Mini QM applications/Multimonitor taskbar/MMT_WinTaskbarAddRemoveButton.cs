 /
function action hwnd ;;action: 1 add, 2 delete

 Adds or deletes taksbar button for window whose handle is hwnd.
 Note: Windows adds the deleted button when the window is activated.


MMTVAR- v

if(!v.itblist)
	v.itblist._create(CLSID_TaskbarList)
	v.itblist.HrInit

sel action
	case 1 if(IsWindowVisible(hwnd)) v.itblist.AddTab(hwnd)
	case 2 v.itblist.DeleteTab(hwnd)

err+
