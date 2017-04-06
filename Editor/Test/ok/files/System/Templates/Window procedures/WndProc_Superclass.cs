
 This template contains a window procedure for a new control window class that extends an existing class using superclassing.
 Superclassing is used to create a new control window class that has features of an existing class (base class) + some new features and/or changes. Read more in function __RegisterWindowClass.Superclass help.
 At the beginning there is code that registers the class, and a template function help section. Remove it if don't need. Or edit it: change class names, type name, variable name, etc.

 ___________________________________

 Registers and implements window class "MyClass2".
 A control of this class ....
 Call this function when QM starts, eg in function "init2". In exe - in main function. Or call on demand, where need it.
 Then controls of this class can be used in dialogs and other windows.
 To add a control to a dialog, in Dialog Editor click "Other controls", type "MyClass2", OK.


__RegisterWindowClass+ __MyClass2
if(__MyClass2.atom) ret ;;already registered
__MyClass2.Superclass("Edit" "MyClass2" &sub.WndProc_Superclass 4)
QmSetWindowClassFlags "MyClass2" 1|2 ;;flags: 1 use dialog variables, 2 use dialog definition text, 4 disable text editing in Dialog Editor, 8 supports "WM_QM_DIALOGCONTROLDATA"


#sub WndProc_Superclass
function# hwnd message wParam lParam

type __WINDOW_DATA_MyClass2 hwnd sample variables
__WINDOW_DATA_MyClass2* d
if(message=WM_NCCREATE) SetWindowLong hwnd __MyClass2.baseClassCbWndExtra d._new; d.hwnd=hwnd
else d=+GetWindowLong(hwnd __MyClass2.baseClassCbWndExtra); if(!d) ret sub.CallBaseWindowProc(hwnd message wParam lParam)

 OutWinMsg message wParam lParam ;;uncomment to see received messages

 sample message handling
sel message
	case WM_CREATE sub.OnCreate(hwnd d +lParam)
	case WM_SETTEXT sub.OnSetText(hwnd d +lParam)
	 ...

int R=sub.CallBaseWindowProc(hwnd message wParam lParam)

 sel message
	 case ...

if(message=WM_NCDESTROY) d._delete
ret R


 Sample message handler functions.
 Note: This is a Unicode class. In text messages text is UTF-16 (word*).

#sub OnCreate
function# hwnd __WINDOW_DATA_MyClass2&d CREATESTRUCTW&c

str s.ansi(c.lpszName)
out F"WM_CREATE: '{s}'" ;;note: depends on QmSetWindowClassFlags flag 2
d.sample=1234


#sub OnSetText
function# hwnd __WINDOW_DATA_MyClass2&d word*textUtf16

str s.ansi(textUtf16)
out F"WM_SETTEXT: '{s}'"
out d.sample


 Other functions.

#sub CallBaseWindowProc
function# hwnd message [wParam] [lParam]

ret CallWindowProcW(__MyClass2.baseClassWndProc hwnd message wParam lParam)
