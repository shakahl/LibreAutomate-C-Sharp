 /dlg_tabbed_web_browser2
function IDispatch'pDisp `&URL SHDocVw.IWebBrowser2'wbDTWB2

 This function is called when a web page loaded in a tab.
 Sets tab title.

ARRAY(TWB_TAB)- ta

 get hwnd of host ActiveX control of this web page
int hwnd i
IOleWindow ow=+wbDTWB2; ow.GetWindow(hwnd)
hwnd=GetParent(hwnd)

 get tab index
i=GetWinId(hwnd)-100; if(i<0 or i>=ta.len) ret

 get page title
MSHTML.IHTMLDocument2 doc=wbDTWB2.Document
str title=doc.title
#if QMVER>=0x02030307
title.LimitLen(40 1)
#endif

 set tab title
TCITEMW ti.mask=TCIF_TEXT
ti.pszText=@title
SendMessage id(3 GetParent(hwnd)) TCM_SETITEMW i &ti

err+ ;;QM bug: QM crashes if unhandled error here
