\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib ATLNEWGRIDLib {1A6B8660-58C4-11D3-B221-006097FEBF00} 1.0

if(!ShowDialog("AX_AtlNewGrid" &AX_AtlNewGrid)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 224 112 "ATLNEWGRIDLib.Grid"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ATLNEWGRIDLib.Grid gr3._getcontrol(id(3 hDlg))
	
	stdole.IPictureDisp p1 p2
	_s.expandpath("$qm$\copy.ico")
	 _s.expandpath("$qm$\bitmap1.bmp") ;;works too
	OleLoadPictureFile(_s &p1)
	_s.expandpath("$qm$\email.ico")
	OleLoadPictureFile(_s &p2) ;;why does not load some icons?
	 out p2
	
	gr3.RowCount=8
	gr3.ColumnCount=6
	int r c; str s
	for r 0 gr3.RowCount
		gr3.RowHeight(r)=16
		for c 0 gr3.ColumnCount
			if(r)
				if(c) s.from(" " r-1); s[0]='A'+c-1
				else s=r-1
			else if(c) s=" "; s[0]='A'+c-1
			else s=""
			gr3.Text(r c)=s
			if(r and c) gr3.CellBgColor(r c)=ColorAdjustLuma(RandomInt(0 0xffffff) 500 1)
		
		if(r)
			gr3.Image(r 0)=5 ;;this works, but the imagelist is built into the control and cannot be changed without recompiling the control
			 gr3.Cell(r 0).Picture=+p1 ;;does not work
			gr3.Cell(r 1).Picture=+p1 ;;works
			gr3.Cell(r 2).Picture=+p2 ;;works
	
	for c 0 gr3.ColumnCount
		gr3.ColumnWidth(c)=50
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	gr3._getcontrol(id(3 hDlg))
	ATLNEWGRIDLib.IGridCell ce=gr3.Cell(2 1)
	out ce.Text
	case IDCANCEL
ret 1
