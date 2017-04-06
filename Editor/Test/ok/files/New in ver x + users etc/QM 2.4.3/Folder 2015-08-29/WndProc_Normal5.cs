
 This template contains a window procedure for a new window class that you can register with functions RegisterClassEx, __RegisterWindowClass.Register or MainWindow.
 At the beginning there is code that registers the class, and a template function help section. Remove it if don't need. Or edit it: change class name, type name, variable name, etc. You can instead use function MainWindow; it registers class and runs message loop.

 ___________________________________

 Registers and implements window class "MyClass1".
 A window/control of this class ....
 Call this function when QM starts, eg in function "init2". In exe - in main function. Or call on demand, where need it.
 Then controls of this class can be used in dialogs and other windows.
 To add a control to a dialog, in Dialog Editor click "Other controls", type "MyClass2", OK.


__RegisterWindowClass+ __MyClass1
if(__MyClass1.atom) ret ;;already registered
__MyClass1.Register("MyClass1" &sub.WndProc_Normal 4)
 QmSetWindowClassFlags "MyClass1" 1|2 ;;flags: 1 use dialog variables, 2 use dialog definition text, 4 disable text editing in Dialog Editor, 8 supports "WM_QM_DIALOGCONTROLDATA". You can use this for control classes.
memset &__MyClass1 0 sizeof(__MyClass1)


#sub WndProc_Normal
function# hwnd message wParam lParam

 Remove this code if don't need.
type __WINDOW_DATA_MyClass1 hwnd ;;add more member variables
__WINDOW_DATA_MyClass1* d
if(message=WM_NCCREATE) SetWindowLong hwnd __MyClass1.baseClassCbWndExtra d._new; d.hwnd=hwnd
else d=+GetWindowLong(hwnd __MyClass1.baseClassCbWndExtra); if(!d) ret DefWindowProcW(hwnd message wParam lParam)

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_CREATE
	
	case WM_DESTROY
	 PostQuitMessage 0 ;;enable this line if the thread must end when this window closed
	
	 ...


int R=DefWindowProcW(hwnd message wParam lParam)
 If the class is not Unicode, use DefWindowProc (without W) instead. __RegisterWindowClass registers only Unicode classes.

 sel message
	 case ...

if(message=WM_NCDESTROY) d._delete
ret R
