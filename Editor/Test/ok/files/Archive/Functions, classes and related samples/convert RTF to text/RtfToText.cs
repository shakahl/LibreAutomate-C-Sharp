 /
function $rtf str&text [flags] ;;flags: 1 rtf is file

 Converts RTF text to simple text.

 rtf - RTF text or file path.
 text - receives simple text.


int hwnd=CreateWindowExW(0 L"RichEdit20W" 0 WS_CHILD|ES_MULTILINE 0 0 0 0 HWND_MESSAGE 0 _hinst 0) ;;note: need WS_CHILD, else getwintext gets empty text

if flags&1
	if(!RichEditLoad(hwnd rtf)) end ERR_FAILED
else if !empty(rtf)
	SETTEXTEX t
	if(!SendMessageW(hwnd EM_SETTEXTEX &t rtf)) end ERR_FAILED

text.getwintext(hwnd)
DestroyWindow hwnd
