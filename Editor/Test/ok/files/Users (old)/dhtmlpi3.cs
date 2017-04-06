\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 declare type and variable for "dialog scope" data
type DHTML_DIALOG_DATA VARIANT'doc txt hicon

DHTML_DIALOG_DATA _d
DHTML_DIALOG_DATA& d=_d
 d.doc="C:\apache2triad\htdocs\emis\varpass.htm"
 d.doc=_s.expandpath("$desktop$\index.html")

if(!ShowDialog("dhtmlpi3" &dhtmlpi3 0 0 0 0 0 &d)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 370 167 "QM DHTML Editor"
 7 Button 0x54012003 0x0 314 2 44 12 "Edit mode"
 5 Button 0x54032000 0x0 170 0 48 14 "HTML"
 3 Static 0x54000000 0x0 70 3 16 10 "Size"
 9 ComboBox 0x54230243 0x0 88 1 28 213 "Textsize"
 6 ToolbarWindow32 0x5400000D 0x0 2 0 364 14 ""
 4 ActiveX 0x54000000 0x10 2 14 364 148 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2010805 "" ""

ret
 messages
if(message=WM_INITDIALOG) DT_Init(hDlg lParam)
&d=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG
	d.hicon=GetIcon("qm_red.ico"); SendMessage hDlg WM_SETICON 0 d.hicon
	SHDocVw.WebBrowser dh4._getcontrol(id(4 hDlg))
	 dh4._setevents("dh4__DHTMLEditEvents")
	
	 str template.getmacro("dhtml_template")
	str template=
 about:
 <style type="text/css">
 BODY {
 background: #ffffff;
 color: #000000;
 margin: 0px 0px 0px 0px;
 font-family: Arial, Helvetica, sans-serif;
 font-size: 12px;
 color:#666666;
 width: 640px;
 overflow: auto;
 }
 </style>
 <body>
 This is a test
 </body>
	template.setwintext(id(4 hDlg))
	MSHTML.IHTMLDocument2 doc	
	
	but id(7 hDlg) ;;edit mode by default

	int styles=WINAPI.TBSTYLE_FLAT
	str icons labels; for(_i 0 4) labels+"[]"; icons.formata("%s,%i[]" "editor.icl" _i)
	DT_TbAddButtons id(6 hDlg) 1001 labels icons styles|WINAPI.TBSTYLE_LIST 1
	TO_CBFill(id(9 hDlg) "1[]&2[]3[]4[]5[]6") ;;tip: make copy of this function, as well as for any other function that is not shown in the main popup list when you press .

	ret 1
	case WM_DESTROY
		DT_DeleteData(hDlg)
		DestroyIcon d.hicon
	case WM_COMMAND goto messages2
ret
 messages2
dh4._getcontrol(id(4 hDlg))
doc=dh4.Document
sel wParam
	case 7
	_s=iif(but(lParam) "On" "Off")
	doc.designMode=_s; err mes "failed" ;;once was exception

	case CBN_SELENDOK<<16|9 doc.execCommand("FontSize" 0 CB_SelectedItem(lParam))
	case 1001: doc.execCommand("Bold" 0)
	case 1002: doc.execCommand("Italic" 0)
	case 1003: doc.execCommand("Underline" 0)
	case 1004 doc.execCommand("CreateLink" TRUE)
 
	case 5
	MSHTML.IHTMLDocument3 doc3=+doc
	str html=doc3.documentElement.outerHTML
	ShowText "" html
	
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1

 err+ mes "Error: %s[][9]in: %s" "dhtml editor" "i" _error.description _error.line