 /
function# iid FILTER&f

 In some windows, in filter-function (in mouse hook), the window has 0 acc children.
 FromWindow gets object, but its ChildCount is 0.
 FromMouse gets null.
 Some such windows are: .NET windows, Windows 10 folder windows.


 int w=win("Keyboard Layout Creator 1.4 - 'Layout01 Description'" "WindowsForms10.Window.8.app.0.378734a")
 Acc a.Find(w "PUSHBUTTON" "Caps" "class=*.BUTTON.*[]wfName=VK_CAPITAL" 0x1005)
 out a.a

Acc b.FromWindow(f.hwnd)
 Acc b.FromWindow(f.hwnd OBJID_CLIENT)
 Acc b.FromMouse
out b.a
out b.ChildCount ;;0

 out
 ShowAccDescendants f.hwnd

ret -2

 ret iid	;; run the macro.
 ret macro	;; run other macro. Here 'macro' is its id or name.
 ret 0		;; don't run any macros.
 ret -1		;; don't run any macros but eat the key. Eg if the filter function started a macro using mac.
 ret -2		;; don't run this macro. Other macros with the same trigger can run.
