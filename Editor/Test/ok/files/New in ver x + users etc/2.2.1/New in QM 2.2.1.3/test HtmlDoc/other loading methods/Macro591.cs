 this works, but cannot be used because requires UTF-16 string

 With IE 5, QM crashes with some pages.
 Woks well if using IE instance or web browser control.
 If using IE instance, would activate IE, therefore better use control.


int ie=CreateWindowEx(0 "ActiveX" "{8856F961-340A-11D0-A96B-00C04FD705A2}" WS_POPUP 0 0 0 0 0 0 _hinst 0)
IDispatch wb._getcontrol(ie)

SHDocVw.WebBrowser b=wb
b.Silent=-1 ;;disable script error boxes

_s="about:blank"; _s.setwintext(ie)
MSHTML.IHTMLDocument2 d=wb.Document
 also needs to wait while busy, or sometimes d will be 0
out d

str s.getfile("$desktop$\html.txt")
ARRAY(VARIANT) a.create(1)
a[0]=s
d.write(a)

1
DestroyWindow ie

