\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str lb3
lb3="one[]two[]three"
if(!ShowDialog("dlg_listbox_hover_tooltips" &dlg_listbox_hover_tooltips &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ListBox 0x54230101 0x200 12 18 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
#compile __CToolTip
CToolTip-- t_tt
sel message
	case WM_INITDIALOG
	int-- t_tt_item=-1
	ARRAY(str)-- t_tt_text="one tooltip[]two tooltip[]three tooltip"
	t_tt.Create(hDlg)
	t_tt.AddTool(hDlg 3 "-")
	
	case WM_SETCURSOR
	if GetDlgCtrlID(wParam)=3
		POINT p; xm p wParam 1
		_i=SendMessage(wParam LB_ITEMFROMPOINT 0 MakeInt(p.x p.y))
		if _i<t_tt_text.len and _i!=t_tt_item
			t_tt.AddTool(hDlg 3 t_tt_text[_i] 2)
			t_tt.OnWmSetcursor(wParam lParam)
		t_tt_item=_i
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
