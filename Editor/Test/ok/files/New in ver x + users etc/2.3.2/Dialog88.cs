\Dialog_Editor
/exe
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str ax3SHD;;=""
 IntGetFile "http://www.google.com" ax3SHD
 IntGetFile "http://www.meteo.lt/oru_prognoze.php" ax3SHD
 out ax3SHD
 ax3SHD="$desktop$\ąčę ٺ\test.txt"

 ax3SHD.setfile("$temp$\test.html")
 ax3SHD="$temp$\test.html"

if(!ShowDialog("" &Dialog88 &controls)) ret
 <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
 <meta http-equiv="Content-Type" content="text/html; charset=windows-1257">

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x8 0 0 545 324 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 546 310 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 310 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 ret
	 g1
	
	_s=
	 <html><head>
	 </head><body>
	 abcd
	 ąčęž
	 ب₪
	 abcd
	 </body></html>
	
	 _s.getfile("$temp$\qm\tools.htm")
	
	 IntGetFile "http://www.google.com" _s
	 IntGetFile "http://www.meteo.lt/oru_prognoze.php" _s
	
	_s=
	 <html>
	 <head>
	 </head>
	 <frameset>
	 <frame src="http://www.quickmacros.com/forum">
	 </frameset>
	 </html>
	
	_s=
	 <html>
	 <head>
	 <script type="text/javascript" src="q:\app\htmlhelp\h.js"></script>
	 <script type="text/javascript">
	 function Google(searchText)
	 {
	 window.open('http://www.google.com/search?q=' + escape(searchText));
	 }
	 </script>
	 <link rel=StyleSheet href="q:\app\htmlhelp\QM-Help.css">
	 </head>
	 <body>
	 <span class="red">red text</span><br>
	 <a href="javascript:Google('quick macros');">google qm</a>
	 </body></html>
	HtmlToWebBrowserControl id(3 hDlg) _s 15
	
	
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	MSHTML.IHTMLDocument2 doc=we3.Document
	 out doc.charset
	out doc.readyState
	out doc.all.length
	
	 we3.Navigate(_s.expandpath("$temp$\qm\tools.htm"))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 goto g1
	case IDOK
	case IDCANCEL
ret 1
