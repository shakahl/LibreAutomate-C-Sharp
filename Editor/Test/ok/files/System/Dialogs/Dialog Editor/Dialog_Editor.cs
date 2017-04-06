function [flags] ;;flags: 1 new dialog.

type ___DE_ADDCONTROL ~cls style exstyle cx cy ~txt
type ___DE_CONTROL cid style exstyle ~txt ~tooltip flags ;;flags: 1 horz line, 2 vert line
type ___DE_UNDO ~dd fid page
type ___DE_CLIPBOARD ___DE_CONTROL'c ~cls ~txt style exstyle x y wid hei ;;used to copy/paste controls

class __DE
	_hwnd ;;Dialog Editor
	_hpane ;;left pane (child dialog)
	_htb ;;toolbar
	_hform ;;dialog
	_hsel ;;selected control in hform, or hform
	_hselName ;;variable name of hsel control, in hpane
	_hselText ;;text of hsel control, in hpane
	_htv ;;'Add control' treeview, in hpane
	_hmark ;;small red rect over hsel
	ARRAY(___DE_ADDCONTROL)_aadd ;;new control props for all treeview non-folder items
	ARRAY(___DE_CONTROL)_ac ;;form and controls. Eg need to store text, style etc that may be different than of the control window.
	!_updateCode ~_userType ~_userIdsVarAdd ~_pageMap _page ;;dialog settings, multi-page support
	_grid _bgColor ~_userClassesVarAdd ;;Dialog Editor settings
	__GdiHandle'_brushMark __GdiHandle'_brushBack __ImageList'_il ;;brushes, images for toolbar and treeview
	_ddMacro ;;id of dialog macro
	~_ddSub ;;empty or name of dialog sub-function
	ARRAY(___DE_UNDO)_u _upos ;;Undo
	_dbx _dby ;;to convert pixels to/from dialog units
	_xform _yform ;;hform position in hmain
	_xDlg _yDlg ;;dialog x y specified in DD
	_dialogFlags ;;dialog flags from dialog definition, 1 if Unicode
	_edge ;;used to move/size in single axis
	!_save ;;dialog modified
	!_textChanged ;;editing hselText, don't create multiple Undo points
	!_arrowMovSiz ;;moving/sizing with arrow keys, don't create multiple Undo points
	!_newDialog ;;true if DE started to edit new dialog
	!_qmgridHelp ;;show grid control help once
	!_qmcbHelp ;;show QM_ComboBox control help once
	!_qmeditHelp ;;show QM_Edit control help once
	__RegisterHotKey'_hkCapture ;;hotkey to capture alien controls
	~_dialogClass ;;custom class or empty
	_styleFlags ;;used by _Style

if flags&1=0
	int h=win("Dialog Editor" "QM_DE_class")
	if h and SendMessage(h WM_APP 0 0)=qmitem
		act h; err
		ret

lpstr icon="$qm$\dialog.ico"
MainWindow("Dialog Editor" "@QM_DE_class" &sub.WndProc 0 0 500 300 WS_CAPTION|WS_SYSMENU|WS_THICKFRAME|WS_MINIMIZEBOX|WS_VISIBLE|WS_CLIPCHILDREN _hwndqm +flags WS_EX_APPWINDOW icon icon 4)


#sub WndProc
function# hwnd message wParam lParam

__DE* d
if(message=WM_NCCREATE) SetWindowLong hwnd 0 d._new
else d=+GetWindowLong(hwnd 0); if(!d) ret DefWindowProcW(hwnd message wParam lParam)

int R=d.WndProc(hwnd message wParam lParam)

if(message=WM_NCDESTROY) SetWindowLong hwnd 0 d._delete
ret R
