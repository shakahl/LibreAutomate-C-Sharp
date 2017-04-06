 /
function hwndctrl [modifiers] ;;modifiers: 1 Shift, 2 Ctrl, 4 Alt, 8 Win

 Use this function to subclass a control to enable dialog moving.
 Call it eg under WM_INITDIALOG.


SetProp hwndctrl "qm_wndproc" SubclassWindow(hwndctrl &DialogDragSubclassProc)
SetProp hwndctrl "qm_modifiers" modifiers
