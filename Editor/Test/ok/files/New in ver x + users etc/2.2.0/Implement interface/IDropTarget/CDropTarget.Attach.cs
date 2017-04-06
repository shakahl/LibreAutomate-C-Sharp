function hwnd
m_effect=0
m_hwnd=hwnd
if(!hwnd) ret
RevokeDragDrop(hwnd)
dll ole32 [RegisterDragDrop]#__RegisterDragDrop hwnd !*pDropTarget
__RegisterDragDrop(hwnd &this)
