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

 <html><head></head><body>
 </body></html>

str controls = "3"
str ax3SHD
 ax3SHD=""
 goto g1
ax3SHD.expandpath("$temp$\qm\tools.htm")
 if(!dir(ax3SHD))
	mkdir "$temp$\qm"
	 _s="<html><body><a href='javascript:alert(1);'>javascript</a></body></html>"
	_s=html
	_s.setfile(ax3SHD)
 ax3SHD.expandpath("$qm$/web/rc.html")
 ax3SHD.all
 ax3SHD="C:\Users\G\Documents\price change.csv"
 ax3SHD="http://www.quickmacros.com"
 ax3SHD=html
 g1
if(!ShowDialog("dlg_tools" &dlg_tools &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 282 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 266 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 268 48 14 "Button"
 5 Button 0x54032000 0x0 108 268 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

ret
 messages
MSHTML.IHTMLDocument2 d
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser tw
	tw._getcontrol(id(3 hDlg))
	 tw._setevents("tw_DWebBrowserEvents2")
	 PostMessage hDlg WM_COMMAND 4 0
	
	 d=tw.Document
	 out d.readyState
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
tw._getcontrol(id(3 hDlg))
sel wParam
	case 5
	_s="http://www.quickmacros.com"; _s.setwintext(id(3 hDlg))
	 _s="$temp$\qm\tools.htm"; _s.setwintext(id(3 hDlg))
	case 4
	
	 Q &q
	  _s=""; _s.setwintext(id(3 hDlg))
	 html.setwintext(id(3 hDlg))
	 Q &qq; outq
	 ret
	
	 _s.getfile("$temp$\qm\tools.htm"); _s+"<p>ttt</p>";
	 Q &q
	 _s.setfile("$temp$\qm\tools.htm")
	 Q &qq
	 tw.Refresh; 0
	 Q &qqq; outq ;;20% slower than setwintext
	 ret
	
	d=tw.Document; err _s=""; _s.setwintext(id(3 hDlg)); d=tw.Document; err ret
	Q &q
	 d.body.innerHTML=""
	html+"<p>hhhh</p>"
	d.body.innerHTML=html; 0
	Q &qq; outq ;;40% faster than setwintext.
	
	 note: cannot replace whole html, only body.
	    This throws error: MSHTML.IHTMLDocument3 d=tw.Document; d.documentElement.innerHTML="<body></body>"
	    This too: d.all.item(0).innerHTML="<body></body>" ;;html
	    This too: d.all.item(1).innerHTML="<title>tt</title>" ;;head
ret 1

 ________________________

 I tested various ways of loading HTML into web browser control.
 Can be used 3 ways. Speed similar.

 1. setwintext(HTML).
    Cons: Sets Internet security zone, therefore something may not work (javascript (if disabled in IE options), opening local files, etc).
    Pros: Does not use files.

 2. At first setwintext(templatefile). Then setfile(temporaryfile); Refresh.
    Cons: Uses template file. Uses temporary file. Slowest.
    Pros: Can use external CSS file, specified in template file.

 3. At first setwintext(templatefile). Then body.innerHTML=HTML.
    Cons: Uses template file. Replaces only body.
    Pros: Fastest. Can use external CSS file, specified in template file. Does not write to files, only reads.

 The 1 can be used if you don't rely on javascript etc. But be careful.

 The 2 can be used if you need to replace whole HTML.

 The 3 is the best if you need to replace only body.

 ________________________

 How to know when a link or form submit button clicked?

 Use BeforeNavigate2 event. Then you can cancel browser's action and do what you like.

 However the event is not always fired on form submit.
    Solution: Don't use true submit (Submit button). Instead use Submit link, and in BeforeNavigate2 handler get form control values using html functions.
