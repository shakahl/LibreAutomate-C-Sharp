 /
function! hwnd cbFunc [cbParam]

 Sets an user-defined function to be called to draw on a control in a dialog or other window.
 Returns: 1 success, 0 failed.

 hwnd - control handle.
 cbFunc - address of callback function, like &MyDrawControlFunc.
   function hWnd hdc RECT&r cbParam
   Tip: menu File -> New -> Templates -> Callback -> Callback_DrawOnControl.
 cbParam - some value to pass to the callback function.

 REMARKS
 The callback function is called whenever the control receives WM_PAINT message, after calling control's window procedure, therefore can draw on top. It can use Windows GDI or GDI+ functions.
 This function subclasses the control to intercept WM_PAINT messages sent to the control.
 This function also can draw directly on dialog, but instead you can use the standard way (case WM_PAINT/BeginPaint/EndPaint) or DT_SetBackgroundColor/DT_SetBackgroundImage.


#ifndef SetWindowSubclass ;;< QM 2.3.5
dll comctl32
	[410]#SetWindowSubclass hWnd pfnSubclass uIdSubclass dwRefData   ;;pfnSubclass: function# hWnd uMsg wParam lParam uIdSubclass dwRefData
	[411]#GetWindowSubclass hWnd pfnSubclass uIdSubclass *pdwRefData
	[412]#RemoveWindowSubclass hWnd pfnSubclass uIdSubclass
	[413]#DefSubclassProc hWnd uMsg wParam lParam
#endif

if(IsBadCodePtr(cbFunc)) end ERR_BADARG

type __DT_DOC cbFunc cbParam
__DT_DOC* d._new; memcpy d &cbFunc 8

int R=SetWindowSubclass(hwnd &DT_DOC_WndProc 1 d)
if(!R) d._delete
ret R!0
