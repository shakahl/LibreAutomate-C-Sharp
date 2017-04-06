Delete

#exe addactivex "SHDocVw.WebBrowser"

if m_flags&2
	m_hax=CreateWindowEx(0 "ActiveX" "SHDocVw.WebBrowser" WS_POPUP 0 0 0 0 HWND_MESSAGE 0 _hinst 0)
	_s.setwintext(m_hax)
	SHDocVw.WebBrowser wb._getcontrol(m_hax)
	d=wb.Document
else
	d._create(uuidof(MSHTML.HTMLDocument))

if m_flags&1
	d.designMode="On"
	 info:
	 We use this to disable scripts etc.
	 Don't off, or will not work.
	 Almost same speed.
	 Inserts <META name=GENERATOR... Slightly modifies HTML in some pages.
	 Four our purposes it should be OK. Too difficult to implement true script disabling.

d3=+d

err+ end _error
