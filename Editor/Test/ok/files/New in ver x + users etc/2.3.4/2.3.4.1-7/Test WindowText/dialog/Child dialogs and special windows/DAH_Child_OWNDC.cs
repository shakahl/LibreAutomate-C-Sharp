 /dlg_apihook
function hDlg

__RegisterWindowClass+ g_atomOWNDC; if(!g_atomOWNDC.atom) g_atomOWNDC.Register("qm_OWNDC" &DAH_WndProc_OWNDC 0 0 CS_OWNDC)

 CreateControl(0 "qm_OWNDC" "OWNDC" 0 0 500 200 80 hDlg 101)
CreateControl(0 "qm_OWNDC" "OWNDC" WS_CAPTION 0 500 200 80 hDlg 101)
