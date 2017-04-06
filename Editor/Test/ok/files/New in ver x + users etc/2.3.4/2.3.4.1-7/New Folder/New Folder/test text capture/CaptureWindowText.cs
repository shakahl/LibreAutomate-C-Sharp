 /
function# WTI*&arr hwnd [flags] [RECT*r]

 Gets all window text.


if(flags&WT_WAIT) end ERR_BADARG

ITextCapture- __t_tc=CreateTextCapture

int n=__t_tc.Capture(arr hwnd flags r)

err+ int e=1
__t_tc.End()
if(e) end _error
ret n
