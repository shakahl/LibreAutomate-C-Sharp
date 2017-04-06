function hwnd mouseButton ;;mouseButton: 1 left, 2 right, 3 any

 Sets drag-drop parameters.
 Call once, before the loop that calls Next().

 hwnd - handle of a window where the drag/drop operation will start. In dialog procedure you can use hDlg or a control handle.
   The window must belong to current thread. It (or its parent) should be the active window.
 mouseButton - drop when this mouse button released: 1 left, 2 right, 3 any.


m_hwnd=hwnd
m_mouseButton=mouseButton
