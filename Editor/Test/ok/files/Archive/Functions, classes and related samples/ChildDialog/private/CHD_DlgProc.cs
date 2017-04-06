 /
function# hDlg message wParam lParam

ChildDialog* p=+GetProp(hDlg "this")
if(p) ret p.__DlgProc(hDlg message wParam lParam)
