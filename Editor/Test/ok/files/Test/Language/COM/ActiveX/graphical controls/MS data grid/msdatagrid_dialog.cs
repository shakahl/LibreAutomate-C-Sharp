\Dialog_Editor
function# hDlg message wParam lParam

 typelib MSDataListLib {F0D2F211-CCB0-11D0-A316-00AA00688B10} 1.0
 typelib MSFlexGridLib {5E9E78A0-531B-11CF-91F6-C2863C385E30} 1.0
typelib MSDataGridLib {CDE57A40-8B86-11D0-B3C6-00A0C90AEA82} 1.0


if(hDlg) goto messages
if(!ShowDialog("msdatagrid_dialog" &msdatagrid_dialog)) ret

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 421 313 "datagrid"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 ActiveX 0x54000000 0x0 12 4 248 101 "MSDataGridLib.DataGrid"
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	Database db4.Open(db4.CsAccess("$personal$\db1.mdb"))
	ADO.Recordset rs4; ARRAY(str) a4
	db4.QueryRs("SELECT * FROM Table1" rs4)
	db4.RsGetAll(rs4 a4)
	
	MSDataGridLib.DataGrid da4._getcontrol(id(4 hDlg))
	da4.AllowUpdate=TRUE
	 da4.AllowAddNew=TRUE
	
	 ADO.Recordset rs4._create
	 rs4.Open(@ @ -1 -1 -1)
	 rs4.
	 rs4.Fields.Append("A" ADO.adVariant 0 -1)
	 rs4.Fields.Append("B" ADO.adVariant 0 -1)
	 rs4.Fields.Append("C" ADO.adVariant 0 -1)
	 rs4.AddNew("A" 1)
	 rs4.AddNew("ID" 2)
	 rs4.AddNew
	 rs4.MoveFirst
	
	 int i
	 for(i 0 5)
		 rs4.AddNew()
		 rs4.Fields.Item(0).Value="string"
		 rs4.Fields.Item(1).Value=i
		 rs4.Fields.Item(2).Value="string"

	da4.DataSource=+rs4
	 da4.Text
	
	 typelib StdFormat {6B263850-900B-11D0-9484-00A0C91110ED} 1.0
	 StdFormat.StdDataFormat fm._create
	 fm.Type=StdFormat.fmtCustom
	 fm.Format="''€'' #.##0,00"
	  VARIANT v; v="Yess"; fm.TrueValue=&v ;;some properties require variable
	 da4.Columns.Item(1).DataFormat=+fm

	 da4.RowHeight=PixelsToTwips(40 1)
	 da4.RowHeight=100*26
	 da4.DefColWidth=200*26
	 da4.Columns.Item(0).Width=200*26
	 da4.Columns.Item(0).Width=PixelsToTwips(200 0)
	DT_Init(hDlg lParam)
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	 case WM_DROPFILES mes _s
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
		
		
ret 1
