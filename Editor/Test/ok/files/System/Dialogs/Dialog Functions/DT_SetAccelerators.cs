 /
function! hDlg [$accelMap] ;;accelMap: "id keys[]id keys[]..."

 Adds accelerators to the dialog.
 Returns: 1 success, 0 failed.

 accelMap - list of command id and hotkey pairs. Hotkeys are in QM format.
   Example: "15 Ck[]16 F12[]...". On Ctrl+K will be sent command 15. On F12 - 16.

 REMARKS
 Accelerators are hotkeys that work in active windows of one thread. In this case - in this dialog.
 When user presses an accelerator hotkey, the dialog receives WM_COMMAND message with accelerator id.
 You can call this function to set, change or remove accelerators.
 This function does not replace menu accelerators (ShowDialog or <help>DT_SetMenu</help>). You can use both.

 See also: <MessageLoopOptions>.
 Added in: QM 2.3.5.


int st=GetWinStyle(hDlg)
if(st&WS_CHILD) ret
__DIALOG* d=+GetProp(hDlg +__atom_dialogdata); if(!d) ret

if(d.haccel2) DestroyAcceleratorTable d.haccel2; d.haccel2=0
if(empty(accelMap)) ret
__Accelerators a
d.haccel2=a.Create(accelMap); a.hacc=0; if(!d.haccel2) ret
sub_DT.SetHook
ret 1
