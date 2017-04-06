\Dialog_Editor
function# hDlg message wParam lParam

int i r c nr nc
ARRAY(str)- a ;;thread variable declaration must be always executed

if(hDlg) goto messages

typelib ATLGrid "C:\Documents and Settings\G\Desktop\ATLNewGrid.dll"

 create array for cells
a.create(3 3)
a[0 0]=""; a[1 0]="A"; a[2 0]="B" ;;row 1
a[0 1]=1; a[1 1]="A1"; a[2 1]="B1" ;;row 2
a[0 2]=2; a[1 2]="A2"; a[2 2]="B2" ;;row 3

if(!ShowDialog("DlgATLGrid" &DlgATLGrid)) ret

 display values of editable cells
 for r 1 a.len(2) ;;rows
	 out "Row %i" r
	 for c 1 a.len(1) ;;columns
		 out a[c r]

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x4 0 0 200 62 "ATLGrid.Grid"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	 get control
	ATLGrid.Grid gr3._getcontrol(id(3 hDlg))
	 set cell dimensions
	for(i 0 10)	gr3.ColumnWidth(i)=100
	for(i 0 10)	gr3.RowHeight(i)=18
	 set number of columns and rows
	gr3.ColumnCount=a.len(1)
	gr3.RowCount=a.len(2)
	 set data
	for r 0 a.len(2) ;;rows
		for c 0 a.len(1) ;;columns
			 gr3.Cell(r c).Text=a[c r]
			 a[c r]=gr3.Cell(r c).Text
			ATLGrid.IGridCell cell=gr3.Cell(r c)
			cell.Text=a[c r]
			BSTR b=cell.Text
			BSTR bb=cell.Text
			 a[c r]=b
			out "%i %i" b.pstr bb.pstr ;;bug: cell.Text returns not copy of string, but pointer to internal string, which is not COM standard.
			b.pstr=0
			bb.pstr=0
			 out "%i" cell.Text
			 out "%i" cell.Text
			 a[c r].ansi(cell.Text)
	 
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	 out "------"
	 get control
	 gr3._getcontrol(id(3 hDlg))
	 
	 str s=gr3.Cell(2 2).Text
	 gr3=0
	 for r 0 3 ;;rows
		 for c 0 3 ;;columns
			  out "%i %i" r c
			 ATLGrid.IGridCell cell=gr3.Cell(r c)
			 str s=cell.Text
			 a[c r]=gr3.Cell(r c).Text
	
	 create new array (rows may be added or deleted)
	 nc=gr3.ColumnCount; nr=gr3.RowCount
	   get cells
	 a.create(nc nr)
	 for r 0 a.len(2) ;;rows
		 for c 0 a.len(1) ;;columns
			  out "%i %i" r c
			 ATLGrid.IGridCell cell=gr3.Cell(r c)
			 str s=cell.Text
			 a[c r]=gr3.Cell(r c).Text
	
	DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
