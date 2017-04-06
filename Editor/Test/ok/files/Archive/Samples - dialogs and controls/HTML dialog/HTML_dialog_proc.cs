 /HTML_dialog_sample
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 356 199 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 356 178 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 256 182 48 14 "OK"
 2 Button 0x54030000 0x4 306 182 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 set events for document
	MSHTML.HTMLDocument doc=DT_GetHtmlDocument(hDlg 3)
	doc._setevents("doc_HTMLDocumentEvents")
	
	 get input text element and set focus
	MSHTML.HTMLInputElement te=+doc.getElementById("t")
	doc.focus
	te.focus
	
	 get input button element and set events for it
	MSHTML.HTMLInputElement- btn=+doc.getElementById("b")
	btn._setevents("btn_HTMLInputTextElementEvents")
	
	 Note: HTMLInputElement can be used for any type of input elements. For some element types also can be used specific interfaces, eg HTMLInputTextElement.
	 Note: Use thread variables for html elements for which is called _setevents.
	 Note: After navigating to other page, if these thread variables still must be used, they must be reinitialized, and _setevents called again. If not used, they should be cleared (=0).
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	 get form data on OK
	str- t_form_data
	doc=DT_GetHtmlDocument(hDlg 3)
	HD_sample_get_form_data doc t_form_data
	
	case IDCANCEL
ret 1
