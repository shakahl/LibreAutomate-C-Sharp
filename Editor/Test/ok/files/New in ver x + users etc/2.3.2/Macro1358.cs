dll "qm.exe" SetWebBrowserHtml hwnd $s
dll "qm.exe" !HtmlWrite2 IDispatch'doc $s

out
str HTML
IntGetFile "http://www.quickmacros.com" HTML

MSHTML.IHTMLDocument2 d
IDispatch dd

 d._create(uuidof(MSHTML.HTMLDocument))

str controls = "3"
str ax3SHD=""
int hDlg=ShowDialog("" 0 &controls 0 1)
opt waitmsg 1
0.2

SetWebBrowserHtml id(3 hDlg) HTML
 out HtmlWrite2(dd HTML)

SHDocVw.WebBrowser wb._getcontrol(id(3 hDlg))
d=wb.Document


 d.open("text/html")
 out d.defaultCharset
 out d.charset

interface# IPersist :IUnknown
	GetClassID(GUID*pClassID)
	{0000010c-0000-0000-C000-000000000046}
interface# IPersistStreamInit :IPersist
	IsDirty()
	Load(IStream'pStm)
	Save(IStream'pStm fClearDirty)
	GetSizeMax(ULARGE_INTEGER*pCbSize)
	InitNew()
	{7FD52380-4E07-101B-AE2D-08002B2EC713}

 __Stream t.CreateOnHglobal(HTML len(HTML)+1)
 IPersistStreamInit ps=+dd
  t.ToStr(_s t.GetSize 1); out _s; ret
 ps.InitNew
 ps.Load(t)
 ps=0; t.is=0

 out HtmlWrite2(dd HTML)

 d=dd
 out d.all.length
int i
for i 0 d.all.length
	MSHTML.IHTMLElement el=d.all.item(i)
	out el.tagName

DestroyWindow hDlg

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 136 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "" "" ""
