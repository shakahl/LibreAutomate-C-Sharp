function hDlg [style]

 Call this on WM_INITDIALOG (in dialog procedure) or WM_CREATE (in window procedure or toolbar hook procedure).

 style - tooltip style, declared in CToolTip Help.


m_htt=CreateWindowEx(WS_EX_TRANSPARENT "tooltips_class32" 0 style 0 0 0 0 hDlg 0 _hinst 0);
SendMessage(m_htt TTM_ACTIVATE 1 0);
