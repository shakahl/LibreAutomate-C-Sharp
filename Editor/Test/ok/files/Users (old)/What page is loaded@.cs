 Following are several examples.

 Method 1 - evaluate window name. This works with any browser.
 Let's assume that when the page is loaded, IE window name is "Sign In - Microsoft Internet Explorer"

 find IE window
int h=win("Microsoft Internet Explorer") ;;to create this line, use "Find window or control" dialog
 get window name
str s.getwintext(h) ;;to create this line, use "Window actions" dialog
 if begins with "Sign In" ...
if(s.beg("Sign In")) ;;to learn how to create these lines, read help
	out "yes"
else out "no"


 Method 2 - evaluate address field text (URL). Works with IE6, but slightly modified should work with other browsers too.

 find address control
int h=child(41477 "" "Edit" "Microsoft Internet Explorer" 0x5) ;;to create this line, use "Find window or control" dialog
str s.getwintext(h)
if(s="http://...")
	out "yes"
else out "no"


 Method 3 - evaluate URL. Works only with Internet Explorer.

str s
IeNavigate("" 0 0 s) ;;to create this line, use "Navigate in IE" dialog

if(s="http://...")
	out "yes"
else out "no"


 Method 4 - find some html element in the page, and get its parent document. Works with IE and some other browsers (Avant, MyIE2, Slim).

MSHTML.IHTMLElement el=html("BODY" "" "" win("Internet Explorer") 0 0 0x20) ;;to create these 3 lines, use "Html element actions" dialog
MSHTML.IHTMLDocument2 doc=el.document
str s=doc.url
if(s="http://...")
	out "yes"
else out "no"


 Method 5 - wait for the page. Works only with IE.

IeWait(10 0 "http://...") ;;to create this line, use "Wait" dialog


 Method 6 - open the page and wait until it is loaded. Works only with IE.

int e
e=IeNavigate("http://..." 30) ;;to create this line, use "Navigate in IE" dialog
if(e) ret ;;failed
