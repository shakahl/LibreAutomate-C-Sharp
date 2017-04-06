 /
function hDlg MSHTML.IHTMLDocument2&doc MSHTML.IHTMLElement&el hwnd str&frame update

___EH- dH
str s w tag text html index
int i

if(!update) dH.doc=doc
dH.el=el

 window
if(!update)
	dH.hwnd=GetAncestor(hwnd 2)
	RecGetWindowName dH.hwnd &w
	w.setwintext(id(10 hDlg))
 tag
tag=el.tagName; err out "failed to get element's properties"; ret
tag.setwintext(id(5 hDlg))
 index in
i=sub.IndexIn(+doc el tag)
if(i>=0) s=i
s.setwintext(id(20 hDlg))
opt err 1
 text
text=el.outerText; text.fix(150 2)
SendDlgItemMessage(hDlg 25 CB_SETCURSEL sub.SelectAttribute(el text tag) 0)
text.setwintext(id(11 hDlg))
TO_Check hDlg "19" (text.len and tag~"BODY"=0 and tag~"HTML"=0)
TO_Check hDlg "29" text.len<150
TO_Check hDlg "31" 0
 HTML
html=el.outerHTML; html.fix(300 2)
html.setwintext(id(14 hDlg))
TO_Check hDlg "22" (html.len and !text.len)
TO_Check hDlg "26" html.len<300
TO_Check hDlg "32" 0
 frame
if(!update) SetDlgItemText hDlg 15 frame
 index
index=el.sourceIndex
index.setwintext(id(7 hDlg))
 navigate
SetDlgItemText hDlg 16 ""


#sub IndexIn
function# MSHTML.IHTMLDocument3'doc3 MSHTML.IHTMLElement&el $tag

MSHTML.IHTMLElementCollection col=doc3.getElementsByTagName(tag)
MSHTML.IHTMLElement elic
int i ai(el.sourceIndex) ai2 n0 n1=col.length

rep
	i=n0+n1/2; if(i=n1) ret -1
	elic=col.item(i); if(elic=el) break
	ai2=elic.sourceIndex; if(ai2=ai) break
	if(ai2<ai) n0=i; else n1=i

ret i


#sub SelectAttribute
function# MSHTML.IHTMLElement&el str&s ~tag

if(s.len and tag~"INPUT"=0 and tag~"SELECT"=0) ret
if(tag~"INPUT") str itype=el.getAttribute("type" 2); err
s=""
s=el.getAttribute("id" 2); err
if(s.len) ret 1

if(itype~"submit" or itype~"button" or itype~"reset" or itype~"radio" or itype~"checkbox")
	s=el.getAttribute("value" 2); err
	if(s.len) ret 4
	
s=el.getAttribute("name" 2); err
if(s.len) ret 2
s=el.getAttribute("alt" 2); err
if(s.len) ret 3
s=el.getAttribute("value" 2); err
if(s.len) ret 4
s=el.getAttribute("type" 2); err
if(s.len) ret 5
s=el.getAttribute("title" 2); err
if(s.len) ret 6
s=el.getAttribute("href" 2); err
if(s.len) ret 7
s=el.getAttribute("onclick" 2); err
if(s.len) ret 8
s=el.getAttribute("src" 2); err
if(s.len) ret 9
s=el.getAttribute("classid" 2); err
if(s.len) ret 10
