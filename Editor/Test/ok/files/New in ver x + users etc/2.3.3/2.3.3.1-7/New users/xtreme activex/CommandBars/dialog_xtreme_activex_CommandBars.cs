\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib XtremeCommandBars {555E8FCC-830E-45CC-AF00-A012D5AE7451} 15.0

if(!ShowDialog("dialog_xtreme_activex_CommandBars" &dialog_xtreme_activex_CommandBars 0)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 217 129 "Dialog"
 3 ActiveX 0x44030000 0x0 0 0 20 20 "XtremeCommandBars.CommandBars {5B5CA009-C9D8-49A9-8552-ACD871774128}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

 NOTE: in my examples, ActiveX controls are not licensed. Delete them and add again through QM dialog editor. It will add "data:...." in dialog definition.
 NOTE: in dialog definition, ActiveX is hidden.

ret
 messages
int- t_hdlg ;;you can use it in event functions as dialog handle
XtremeCommandBars.CommandBars co3
sel message
	case WM_INITDIALOG
	t_hdlg=hDlg
	co3._getcontrol(id(3 hDlg))
	co3.AttachToWindow(hDlg); ;;use dialog as parent window. Initially it was QM ActiveX control container control (class "ActiveX", id 3).
	co3._setevents("co3__DCommandBarsEvents")
	co3.LoadDesignerBars(_s.expandpath("$desktop$\Form1.xcb"))
	
	  Maybe you'll find that better is without AttachToWindow.
	  Then need this or similar code to autosize QM ActiveX control container control.
	  Also insert this under case WM_INITDIALOG: SendMessage hDlg WM_SIZE 0 0
	  Also then ActiveX control container control must not be hidden.
	 case WM_SIZE
	 RECT r; GetClientRect hDlg &r; MoveWindow id(3 hDlg) 0 0 r.right r.bottom 1

	case WM_CLOSE DestroyWindow hDlg ;;need this because IDCANCEL disabled
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [IDOK,IDCANCEL] ret ;;don't close on Enter/Esc
ret 1
