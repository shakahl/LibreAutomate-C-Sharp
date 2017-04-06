 /dialog_QM_Tools
function hWnd CREATESTRUCTW&c

str t.ansi(c.lpszName) dd
c.lpszName=0
if t.len
	lpstr s1 s2 s3
	tok t &s1 3 " " 1
	m_type=val(s1); m_flags=val(s2)
m_hwnd=hWnd
m_hparent=GetParent(hWnd)

sel m_type
	case 1
	dd="__ToolsControl_WinDD"
	mw_what=-1
	case else ret

sub_DT.CompileDialog(dd.getmacro(dd) _s &sub.Callback_ &this) ;;creates controls

m_tooltip.Create(hWnd 0 15)

sel m_type
	case 1
	int _lock=iif(m_flags&64 1 val(s3))
	if(_lock) _WinLock(_lock); else if(m_flags&32) _WinSelect(1)
	if(m_flags&16) hid id(510 m_hwnd)


#sub Callback_
function DLGTEMPLATEEX*dt DLGITEMTEMPLATEEX*dit $cls $txt __ToolsControl&x $tooltip

if(dit) x.sub.Callback(dit cls txt tooltip)


#sub Callback c
function DLGITEMTEMPLATEEX*dit $cls $txt $tooltip

RECT r; SetRect &r dit.x dit.y dit.x+dit.cx dit.y+dit.cy
MapDialogRect(m_hparent &r) ;;must be dialog

sel(dit.id) case 500 mw_eHeight=r.bottom

if m_type=1 and m_flags&64 ;;make the "Edit" button small, next to the Window edit control
	sel dit.id
		case 500 GetWinXY m_hwnd 0 0 _i; r.right=_i-(mw_eHeight*7/6)
		case 522 GetWinXY m_hwnd 0 0 _i; r.left=_i-(mw_eHeight*7/6); r.right=_i; r.bottom-r.top+2; r.top=0; txt="..."; tooltip="Edit window properties"
		case else dit.style~WS_VISIBLE

int h=CreateWindowExW(dit.exStyle @cls @txt dit.style r.left r.top r.right-r.left r.bottom-r.top m_hwnd dit.id _hinst 0)
SendMessage(h WM_SETFONT _hfont 0)

sel dit.id
	case 500
		mw_heW=h
		SendMessage h EM_SETCUEBANNER 0 @" Window"
		if(m_flags&256) tooltip="Window.[]Can be win(...), name or +class"
		else tooltip="Window.[]Can be win(...), name, +class or handle.[]If handle isn't a local variable that now is in the macro, enclose it in ( ), like (hwnd).[]If 'Window' selected but this field is empty, will use the active window.[]In dialog procedure, use hDlg as dialog handle; if need control, also enter its id below."
	case 501
		mw_heC=h
		SendMessage h EM_SETCUEBANNER 0 @" Control"
	case 520
		__GdiHandle+ __TC_DragIcon; if(!__TC_DragIcon) __TC_DragIcon=GetFileIcon("$qm$\target.ico" 0 1)
		SendMessageW(h STM_SETICON __TC_DragIcon 0)

if(!empty(tooltip)) m_tooltip.AddControl(dit.id tooltip)
