\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=
 <html><head>
 <script type="text/javascript">
 function blah(x)
 {
 alert(x);
 return 5;
 }
 </script>
 </head>
 <body>test<body/>
 </html>

if(!ShowDialog("dialog_web_browser_invoke_javascript" &dialog_web_browser_invoke_javascript &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 114 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 6 116 48 14 "blah"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040000 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case 4
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	
	MSHTML.IHTMLDocument2 d=we3.Document
	d.parentWindow.execScript("blah(''BLAH'');" "JavaScript")
	
	 or in single line
	 we3.Document.parentWindow.execScript("blah(20);" "JavaScript")
	
	 if need the return value
	 MSHTML.IHTMLDocument2 d=we3.Document
	 d.parentWindow.execScript("R=blah(''BLAH'');" "JavaScript")
	 out d.Script.R
ret 1
