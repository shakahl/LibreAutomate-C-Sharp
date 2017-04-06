 /dlg_apihook
function hDlg

__RegisterWindowClass+ g_atomRTL; if(!g_atomRTL.atom) g_atomRTL.Register("qm_RTL" &DAH_LAYOUTRTL_WndProc 0 0 0 COLOR_INFOBK+1)
 CreateControl(0 "qm_RTL" "RTL" WS_CAPTION 0 400 100 60 hDlg 100)
 CreateControl(WS_EX_LAYOUTRTL "qm_RTL" "RTL" WS_CAPTION 0 400 100 60 hDlg 100)
CreateControl(WS_EX_LAYOUTRTL "qm_RTL" "RTL" 0 0 400 100 60 hDlg 100)

 SetProcessDefaultLayout(LAYOUT_RTL)
 int w=CreateWindowEx(0 "qm_RTL" "Own DC" WS_VISIBLE|WS_CAPTION|WS_SYSMENU 0 0 100 100 0 0 _hinst 0) ;;sets WS_EX_LAYOUTRTL
 opt waitmsg 1; wait 0 WD w
 SetProcessDefaultLayout(0)
