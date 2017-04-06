class __ToolsControl
	m_type m_flags m_hwnd m_hparent
	__Tooltip'm_tooltip
	mw_what mw_lock mw_heW mw_heC mw_eHeight
	!mw_capturing mw_captW mw_captC
	~mw_comments

def __TWM_DRAGDROP WM_USER+400
def __TWM_SETFLAGS WM_USER+401
def __TWM_SETLOCK WM_USER+402
def __TWM_SELECT WM_USER+403
def __TWM_GETCAPTUREDHWND WM_USER+404
def __TWM_GETSELECTED WM_USER+405

def __TWN_DRAGBEGIN WM_USER+500
def __TWN_DRAGEND WM_USER+501
def __TWN_WINDOWCHANGED WM_USER+502

QmSetWindowClassFlags "QM_Tools" 3
#if EXE
__RegisterWindowClass+ __QM_Tools.Register("QM_Tools" &__ToolsControl_WndProc 4)
#else
__RegisterWindowClass+ __QM_Tools.Register("QM_Tools" __CallbackGetAddress(qmitem("__ToolsControl_WndProc")) 4) ;;compile __ToolsControl_WndProc on demand, because we compile/call this func at startup, and the window class is not very small
#endif
