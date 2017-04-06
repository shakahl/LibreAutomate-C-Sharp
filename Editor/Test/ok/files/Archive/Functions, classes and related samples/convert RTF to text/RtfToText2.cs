 /
function $rtf str&text [flags] ;;flags: 1 rtf is file

 Converts RTF text to simple text.

 rtf - RTF text or file path.
 text - receives simple text.

 REMARKS
 The same as RtfToText. Uses EM_STREAMIN instead of EM_EM_SETTEXTEX.


int hwnd=CreateWindowExW(0 L"RichEdit20W" 0 WS_CHILD|ES_MULTILINE 0 0 0 0 HWND_MESSAGE 0 _hinst 0) ;;note: need WS_CHILD, else getwintext gets empty text

if flags&1
	if(!RichEditLoad(hwnd rtf)) end ERR_FAILED
else if !empty(rtf)
	INTLPSTR k.s=rtf; k.i=len(rtf)
	EDITSTREAM e.pfnCallback=&sub.EditStreamCallback; e.dwCookie=&k
	SendMessageW(hwnd EM_STREAMIN SF_RTF &e)

text.getwintext(hwnd)
DestroyWindow hwnd


#sub EditStreamCallback
function# dwCookie !*pbBuff cb *pcb

INTLPSTR& k=+dwCookie
int n=iif(cb<k.i cb k.i)
if(n) memcpy pbBuff k.s n; k.i-n; k.s+n
*pcb=n
