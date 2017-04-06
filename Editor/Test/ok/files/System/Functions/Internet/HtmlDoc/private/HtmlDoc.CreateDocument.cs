function [$_file]

Delete

#exe addactivex "SHDocVw.WebBrowser"

if m_flags&3 or !empty(_file)
	int dlFlags=0x40004400 ;;don't download images, sounds, videos, ActiveX; DLCTL_PRAGMA_NO_CACHE (does not work), DLCTL_SILENT (not necessary if wb.Silent=1). Not used DLCTL_DOWNLOADONLY because does not work.
	if(m_flags&1) dlFlags|0x380 ;;DLCTL_NO_SCRIPTS, DLCTL_NO_JAVA, DLCTL_NO_RUNACTIVEXCTLS
	m_hax=CreateWindowEx(WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE "ActiveX" F"SHDocVw.WebBrowser ambient:{&sub.AmbientProc},{dlFlags}" WS_POPUP 0 0 0 0 HWND_MESSAGE 0 _hinst 0)
	SHDocVw.WebBrowser wb._getcontrol(m_hax)
	wb.Silent=1
	
	if(empty(_file)) _file="about:blank"
	VARIANT f=14 ;;navNoHistory|navNoReadFromCache|navNoWriteToCache ;;MSDN says "not implemented", but works. Actually uses cache, but does not download if not necessary.
	wb.Navigate(_file f)
	
	opt waitmsg 1; 0; rep() if(!wb.Busy) break; else 0.01
	
	d=wb.Document
else
	d._create(uuidof(MSHTML.HTMLDocument))

d3=+d

err+ end _error


#sub AmbientProc
function# dispid `&v param
sel dispid
	case -5512 ;;DISPID_AMBIENT_DLCONTROL
	v=param
	ret 1
