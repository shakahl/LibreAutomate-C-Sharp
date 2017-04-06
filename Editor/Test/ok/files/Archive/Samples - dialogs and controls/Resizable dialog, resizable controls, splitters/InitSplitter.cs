 Registers window class "QM_Splitter".
 Then controls of the class can be used in dialogs and other windows.
 Also declares class DlgSplitter.
 Call this function when QM starts, eg in function "init2". In exe - in main function.

 To add a splitter to a dialog, in Dialog Editor click "Advanced -> Other controls", type "QM_Splitter", OK.
 Then move/resize so that it would be between controls. Also remove WS_TABSTOP and WS_GROUP styles.
 Usually don't need more programming.

 When user drags the splitter, it automatically resizes adjacent controls.
 To move splitter you also can use mov, SetWindowPos or DlgSplitter.SetPos.
 Splitter does not allow to move it outside dialog or adjacent controls. This feature also can be used to: 1. Use hidden controls to limit resizing of real controls. 2. Easily move splitter to its min or max allowed position.

 You can use WM_CTLCOLORSTATIC notification to set splitter color or make transparent (ret GetStockObject(NULL_BRUSH)).

 _________________________________________________________________

 Splitter messages. Don't use. Instead use DlgSplitter class.

def SPM_ENABLE 0x4000
 Enables or disables.
   wParam - 1 enable, 0 disable.
 When disabled, the control does not work as a splitter. Initially enabled.

def SPM_ATTACH 0x4001
 Attaches or detaches controls.
   wParam - 1 attach (find/remeber adjacent controls, those that later will be resized when moving splitter), 0 detach.
 Normally you don't have to use this message. Splitter finds/remembers controls when moved first time.

def SPM_GETBOUNDS 0x4002
 Gets bounds of the area in which splitter can be moved. The values are in client area.
   wParam - 0 or address of int variable that receives min bound.
   lParam - 0 or address of int variable that receives max bound.

def SPM_GETPOS 0x4004
 Gets current position within bounds.
   wParam - 0 or address of int variable that receives max position.

def SPM_SETPOS 0x4005
 Sets current position within bounds.
   wParam - position.

__RegisterWindowClass+ ___splitter_class.Register("QM_Splitter" &SPL_WndProc 0 0 CS_GLOBALCLASS)

class DlgSplitter :DlgControl'__ ;;Manages QM_Splitter control.
