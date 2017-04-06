 /
function# hWnd message wParam lParam

type ___SPLITTER m_hWnd m_hParent !m_disabled !m_horz !m_attached m_hCursor ARRAY(int)m_a1 ARRAY(int)m_a2 POINT'm_ptDrag
___SPLITTER* c

sel message
	case WM_NCCREATE c._new; SetProp(hWnd "QM_Splitter" c)
	case else c=+GetProp(hWnd "QM_Splitter")

_i=c.WndProc(hWnd message wParam lParam)

if(message=WM_NCDESTROY) RemoveProp(hWnd "QM_Splitter"); c._delete

ret _i
