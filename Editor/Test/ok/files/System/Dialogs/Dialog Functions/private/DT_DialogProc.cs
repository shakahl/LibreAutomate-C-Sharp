 /
function# hDlg message wParam lParam

__DIALOG* d; int R

if message=WM_INITDIALOG
	 ShowDialog passes address of __DIALOG as lParam. It uses normal variable, not _new, because need to auto clear on error.
	 Here we create new __DIALOG, copy from __DIALOG of ShowDialog, and zero __DIALOG of ShowDialog to disable clearing it by its dtor.
	 On WM_DESTROY we'll delete our __DIALOG.
	if(!__atom_dialogdata) __atom_dialogdata=GlobalAddAtom("qm_dialogdata")
	SetProp(hDlg +__atom_dialogdata d._new) ;;dwl_user would be faster but unsafe
	memcpy d +lParam sizeof(__DIALOG); memset +lParam 0 sizeof(__DIALOG)
	
	if(d.dlgproc) call(d.dlgproc hDlg WM_CREATE 0 0)
	DT_Init(hDlg d)
	d.flags2|2
	if(d.dlgproc) call(d.dlgproc hDlg message wParam d)
	if(d.flags2&0x100) ret ;;hidden or inactive
	ret !(d.dlgproc and GetFocus)

d=+GetProp(hDlg +__atom_dialogdata); if(!d) ret
if(d.flags2&2=0) goto g1

sel message
	case WM_COMMAND if(!lParam and wParam>>16=1) wParam&0xffff ;;remove accelerator flag to match menu id
	case WM_SETCURSOR if(d.tt) d.tt.OnWmSetcursor(wParam lParam) ;;relays if not hooked, etc
	case else if(d.colors) R=call(d.colors.func d.colors hDlg message wParam lParam); int R2=R ;;call through pointer to make exe smaller when not used

if d.dlgproc
	R=call(d.dlgproc hDlg message wParam lParam)
	sel message
		case WM_COMMAND
		sel wParam
			case [IDOK,IDCANCEL]
			if(!IsWindow(hDlg)) ret R
			if(d.flags2&16) ret R ;;DT_Init was called explicitly, which indicates old version
			if(!R) ret 1 ;;don't close
			if(wParam=IDOK) DT_Ok hDlg
			else DT_Cancel hDlg
			ret 1
		
		case WM_NOTIFY
		if(!R and sub.OnNotify(d hDlg +lParam)) ret 1
		
		case else
		if(R2 and !R) R=R2 ;;if our colors proc on wm_ctlcolorx returned a brush (R2), return it now, but if user's dlgproc returned a brush too, return user's brush instead
else
	sel message
		case WM_COMMAND
		sel wParam
			case IDOK DT_Ok hDlg
			case IDCANCEL DT_Cancel hDlg
			case else ;;if button or menu, close dialog and return id
			if(wParam&0xffff0000) ret 1
			if(lParam) if(!(WinTest(lParam "Button") and GetWinStyle(lParam)&BS_TYPEMASK<2)) ret 1
			DT_Ok hDlg wParam
		ret 1

 g1
sel message
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_QM_ENDTHREAD if(d.flags&1 and !R) DestroyWindow hDlg; ret 1

ret R


#sub OnNotify
function __DIALOG*d hDlg NMHDR*nh
sel nh.code
	case [NM_CLICK,NM_RETURN]
	sel WinTest(nh.hwndFrom "SysLink")
		case 1
		NMLINK& nl=+nh
		if(nl.item.szUrl or nl.item.szID) ret
		call(d.dlgproc hDlg WM_COMMAND nh.idFrom nl.item.iLink)
		ret 1

err+
