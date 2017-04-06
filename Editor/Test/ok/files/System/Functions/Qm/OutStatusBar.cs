 /
function $text

 Sets QM status bar text.

 text - text. If begins with "<>", can contain <help #IDP_F1>tags</help>.

 Added in: QM 2.3.3.


int w

#if EXE
w=win("" "QM_Editor"); if(!w) ret
#else
w=_hwndqm
#endif

SendMessageW id(2204 w) WM_SETTEXT 0 @text
