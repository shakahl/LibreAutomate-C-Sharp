function [RECT&rButton] [RECT&rWindow] [RECT&rClient]
 rButton - button rect in window.
 rWindow - window rect in window (left and top are 0). Other rects are relative to it.
 rClient - client area in window. Includes scrollbars etc, ie the area that we don't draw. We draw only border and button.

RECT r rs rb
GetWindowRect _hwnd &rs
r=rs; OffsetRect &r -r.left -r.top
int cxButton=_GetButtonWidth ;;also inits theme if need
int inflate; if(!_theme) inflate=-_borderWidth; else if(!_borderWidth) inflate=1; else if(_winver<0x600) inflate=-1
rb=r; InflateRect &rb inflate inflate; rb.left=r.right-cxButton
if(&rClient) rClient=r; InflateRect &rClient -_borderWidth -_borderWidth; rClient.right=rb.left
if(&rButton) rButton=rb
if(&rWindow) rWindow=r
