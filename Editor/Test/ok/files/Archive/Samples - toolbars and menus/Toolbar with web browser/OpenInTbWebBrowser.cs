 /
function $url [$tbname]

 Opens url in web browser control on a QM toolbar.

 url - a URL or a local file of supported format (htm, txt, doc, xls, pdf, gif, folder, etc).
 tbname - toolbar name. Can be omitted when this function is called from the toolbar.

 EXAMPLE (toolbar)
  /hook ToolbarExProc_TWWB /siz 500 300 /set 2
 www.quickmacros.com :OpenInTbWebBrowser "http://www.quickmacros.com" * web.ico
 Book1.xls :OpenInTbWebBrowser "$personal$\Book1.xls" * $personal$\Book1.xls


int h
str s=tbname
if(s.len) h=win(s.ucase "QM_toolbar")
else h=val(_command) ;;if called from tb, tb handle is stored in _command
if(!h) ret
h=child("" "ActiveX" h)
if(!h) ret

s=url
s.setwintext(h)

 this also works but not sure is it ok if called from other thread:
 SHDocVw.WebBrowser b._getcontrol(h)
 if(b) b.Navigate(s.expandpath(url))
