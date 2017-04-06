/exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str ax3SHD
IntGetFile "http://www.google.com" ax3SHD
 IntGetFile "http://www.meteo.lt/oru_prognoze.php" ax3SHD
 out ax3SHD
 ax3SHD="$desktop$\ąčę ٺ\test.txt"
 ax3SHD=
 <html><head>
 </head><body>
 abcd
 ąčęž
 ب₪
 abcd
 </body></html>

 ax3SHD.setfile("$temp$\test.html")
 ax3SHD="$temp$\test.html"

if(!ShowDialog("Dialog87" &Dialog87 &controls)) ret
 <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
 <meta http-equiv="Content-Type" content="text/html; charset=windows-1257">

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x8 0 0 545 324 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 546 324 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	MSHTML.IHTMLDocument2 doc=we3.Document
	 out doc.defaultCharset
	 out doc.charset
	 doc.charset="utf-8"
	 out doc.charset
	out doc.readyState
	out doc.all.length
	
	 IntGetFile "http://www.google.com" _s
	 _s="$desktop$\ąčę ٺ\test.txt"
	 _s.setwintext(id(3 hDlg))
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog87
 exe_file  $my qm$\Dialog87.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {B0BC320E-685B-41DB-A3B8-73B2B8C28E79}
 END PROJECT
