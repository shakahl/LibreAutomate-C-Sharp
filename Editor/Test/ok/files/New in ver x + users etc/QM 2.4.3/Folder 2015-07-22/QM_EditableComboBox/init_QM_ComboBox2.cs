
#compile "__QM_ComboList.Show"

class __QM_ComboBox
	_hwnd _hlv _style
	ICsv'_csv _flags
	!_noMessages !_isPressed !_textChanged
	_WM_QM_GETDIALOGVARIABLEDATA

class __QM_ReadonlyComboBox :__QM_ComboBox
	_hedit

class __QM_EditComboBox :__QM_ComboBox
	_theme _borderWidth _stateF _stateB _animTime
	!_themeInited !_isFocused !_isMouseIn !_animate !_styleScrollbars

__RegisterWindowClass+ __rwcQMRCB __rwcQMECB
if(__rwcQMRCB.atom) ret

__rwcQMRCB.Superclass("ComboBox" "QM_ReadonlyComboBox" &sub.WndProc_Readonly 4)
QmSetWindowClassFlags "QM_ReadonlyComboBox" 1|8 ;;flags: 1 use dialog variables, 2 use dialog definition text, 4 disable text editing in Dialog Editor, 8 supports "WM_QM_GETDIALOGVARIABLEDATA"

__rwcQMECB.Superclass("Edit" "QM_EditComboBox" &sub.WndProc_Editable 4)
QmSetWindowClassFlags "QM_EditComboBox" 1|8


#sub WndProc_Readonly
function# hwnd message wParam lParam

__QM_ReadonlyComboBox* d
if message=WM_NCCREATE
	SetWindowLong hwnd __rwcQMRCB.baseClassCbWndExtra d._new
	d._hwnd=hwnd
	d._WM_QM_GETDIALOGVARIABLEDATA=RegisterWindowMessage("WM_QM_GETDIALOGVARIABLEDATA")
else
	d=+GetWindowLong(hwnd __rwcQMRCB.baseClassCbWndExtra)
	if(!d or d._noMessages&1) ret CallWindowProcW(__rwcQMRCB.baseClassWndProc hwnd message wParam lParam)
	if(d._noMessages&2) ret DefWindowProc(hwnd message wParam lParam)

int R=d._WndProc(message wParam lParam)

if(message=WM_NCDESTROY) d._delete
ret R


#sub WndProc_Editable
function# hwnd message wParam lParam

__QM_EditComboBox* d
if message=WM_NCCREATE
	SetWindowLong hwnd __rwcQMECB.baseClassCbWndExtra d._new
	d._hwnd=hwnd
	d._WM_QM_GETDIALOGVARIABLEDATA=RegisterWindowMessage("WM_QM_GETDIALOGVARIABLEDATA")
else
	d=+GetWindowLong(hwnd __rwcQMECB.baseClassCbWndExtra)
	if(!d or d._noMessages&1) ret CallWindowProcW(__rwcQMECB.baseClassWndProc hwnd message wParam lParam)
	if(d._noMessages&2) ret DefWindowProc(hwnd message wParam lParam)

int R=d._WndProc(message wParam lParam)

if(message=WM_NCDESTROY) d._delete
ret R
