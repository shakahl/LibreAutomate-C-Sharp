 /
function hDlg message wParam lParam

 Call this function before sel message in parent dialog procedure.

sel message
	case WM_INITDIALOG
	 create
#opt nowarnings 1
	m_hdlgc=ShowDialog(m_macro m_dlgproc m_controls hDlg 1 WS_CHILD WS_POPUP m_param)
#opt nowarnings 0
	SetProp m_hdlgc "this" &this
	m_subc=SetWindowLong(m_hdlgc DWL_DLGPROC &CHD_DlgProc)
	
	 replace the template control
	RECT rc r; int ht cy
	GetClientRect(m_hdlgc &rc) ;;client before
	ht=id(m_ctrlid hDlg)
	GetWindowRect(ht &r); MapWindowPoints(0 hDlg +&r 2)
	cy=r.bottom-r.top
	MoveWindow m_hdlgc r.left r.top r.right-r.left cy 0
	DestroyWindow ht
	SetWindowLong m_hdlgc GWL_ID m_ctrlid
	
	 set scroll range if needed. It also adds scrollbar.
	SCROLLINFO si.cbSize=sizeof(si)
	si.fMask=SIF_RANGE|SIF_PAGE
	GetClientRect(m_hdlgc &r) ;;client after
	
	 correct horz/vert height if scrollbars will be added
	if(r.right<rc.right) r.bottom-GetSystemMetrics(SM_CYHSCROLL); int corrected=1
	if(r.bottom<rc.bottom)
		r.right-GetSystemMetrics(SM_CXVSCROLL)
		if(!corrected and r.right<rc.right) r.bottom-GetSystemMetrics(SM_CYHSCROLL)
	
	if(r.bottom<rc.bottom)
		si.nMax=rc.bottom
		si.nPage=r.bottom
		SetScrollInfo m_hdlgc SB_VERT &si 0
		m_scmaxv=si.nMax-si.nPage
		m_scpagev=si.nPage
	if(r.right<rc.right)
		si.nMax=rc.right
		si.nPage=r.right
		SetScrollInfo m_hdlgc SB_HORZ &si 0
		m_scmaxh=si.nMax-si.nPage
		m_scpageh=si.nPage
	
	case WM_COMMAND if(wParam=IDOK) DT_GetControls(m_hdlgc)
