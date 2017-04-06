\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str- html=
 <p><a href='http://www.google.com'>google</a></p>
 
 <p><a href='qm:test'>qm:test</a></p>
 
 <form action='qm:test' method='post'>
 <input type='text' name='testtxt' value='12345'>
 <input type='submit' value='qm:test'>
 </form>
 
 <form action='http://www.quickmacros.com/test.php' method='post'>
 <input type='text' name='testtxt' value='12345'>
 <input type='submit' value='quickmacros.com'>
 </form>
 
 <form action='#y' method='post'>
 <input type='text' name='testtxt' value='12345'>
 <input type='submit' value='#y'>
 </form>
 
 <p><a href='javascript:alert(1);'>javascript</a></p>
 
 <p><a href='file:///q:/app/web/index.html'>qm local</a></p>
 
 <form>
 <input type='text' id='cu' value='cccccccccc'>
 <a href='#x'>#x, link submit'</a>
 </form>

str controls = "3"
str ax3SHD
 ax3SHD.expandpath("$temp$\qm\tools.htm")
  if(!dir(ax3SHD))
	 mkdir "$temp$\qm"
	  _s="<html><body><a href='javascript:alert(1);'>javascript</a></body></html>"
	 _s=html
	 _s.setfile(ax3SHD)
 ax3SHD="q:/app/web/rc.html"
if(!ShowDialog("dlg_tools" &dlg_tools &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 241 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 224 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 226 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser tw
	tw._getcontrol(id(3 hDlg))
	tw._setevents("tw_DWebBrowserEvents2")
	PostMessage hDlg WM_COMMAND 4 0
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	tw._getcontrol(id(3 hDlg))
	
	 html.setwintext(id(3 hDlg))
	
	 tw.
	
	_s=""; _s.setwintext(id(3 hDlg))
	MSHTML.IHTMLDocument2 d=tw.Document
	d=d.open("text/html")
	ARRAY(VARIANT) a.create(1)
	a[0]=html
	d.write(a)
	d.close
ret 1
