 /
function'SHDocVw.WebBrowser [$tbname]

 Gets SHDocVw.WebBrowser interface pointer of web browser control on a QM toolbar.

 tbname - toolbar name. Can be omitted or empty when this function is called from the toolbar.


int h
str s=tbname
if(s.len) h=win(s.ucase "QM_toolbar")
else h=val(_command) ;;if called from tb, tb handle is stored in _command
SHDocVw.WebBrowser b._getcontrol(child("" "ActiveX" h))
ret b
err+
