\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD=
 <html><head>
 <script type="text/javascript">
 function OnTimer()
 {
 var d=new Date();
 document.getElementById("notifyQM").value=d.toLocaleTimeString();
 }
 </script>
 </head>
 <body onload="setInterval(OnTimer, 1000);">
 <input type="hidden" id="notifyQM" />
 <p>Text.</p>
 </body></html>
if(!ShowDialog("" &dialog_javascript_callback &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 116 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	
	 get the HTML element and set events for onpropertychange event
	MSHTML.HTMLInputElement- ei
	ei=we3.Document.getElementById("notifyQM")
	ei._setevents("ei_HTMLInputTextElementEvents")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
