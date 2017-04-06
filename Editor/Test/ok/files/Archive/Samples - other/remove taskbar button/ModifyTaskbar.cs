 /
function action hwnd ;;action: 1 add, 2 delete

 Adds or deletes taksbar button for window whose handle is hwnd.
 Note: Windows adds the deleted button when the window is activated.


ITaskbarList t._create(CLSID_TaskbarList)
t.HrInit
sel action
	case 1 t.AddTab(hwnd)
	case 2 t.DeleteTab(hwnd)

err end _error
