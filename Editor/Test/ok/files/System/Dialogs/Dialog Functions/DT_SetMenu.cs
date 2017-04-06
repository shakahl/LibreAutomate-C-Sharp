 /
function! hDlg hmenu [haccel] [flags] ;;flags: 1 don't resize window

 Adds menu to the dialog. Optionally sets accelerators.
 Returns: 1 success, 0 failed.

 REMARKS
 You can call this function to set, change or remove (if hmenu is 0) menu.
 When changing or removing menu, destroys old menu.
 To create menu and accelerators, use <help>DT_CreateMenu</help>.
 Don't need to destroy the menu and accelerators. The dialog will do it.


int st=GetWinStyle(hDlg)
if(st&WS_CHILD) ret
__DIALOG* d=+GetProp(hDlg +__atom_dialogdata); if(!d) ret

RECT rc; GetClientRect hDlg &rc

SetMenu(hDlg hmenu)
if(d.hmenu and d.hmenu!=hmenu) DestroyMenu d.hmenu
d.hmenu=hmenu
if(d.haccel and d.haccel!=haccel) DestroyAcceleratorTable d.haccel
d.haccel=haccel
if(haccel) sub_DT.SetHook

if(flags&1=0)
	AdjustWindowRectEx &rc st hmenu GetWinStyle(hDlg 1)
	if(st&WS_VSCROLL) rc.right+GetSystemMetrics(SM_CXVSCROLL)
	if(st&WS_HSCROLL) rc.bottom+GetSystemMetrics(SM_CYHSCROLL)
	siz rc.right-rc.left rc.bottom-rc.top hDlg

ret 1
