type DLG hDlg

function# DLG'h message wParam lParam

h.SetIcon("icon.ico")
h.Func(3 ...)

h.AnyFunction
{
DLGDATA* d=GetProp(hDlg "_dd"); if(!d) SetProp(hDlg "_dd" p._new)
}
