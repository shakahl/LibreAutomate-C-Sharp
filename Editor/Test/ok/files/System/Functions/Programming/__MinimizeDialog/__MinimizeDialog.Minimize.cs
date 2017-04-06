function hwnd [flags] ;;1 deactivate hwnd

 Minimizes window hwnd.
 If it is owned, minimizes its root owner window. Then owned windows are hidden.


int- t_dragToolFlags; if(t_dragToolFlags&1) ret

int h=hwnd
rep
	if(!h) break
	m_a[]=h
	h=GetWindow(h GW_OWNER)

if(!m_a.len) ret

h=m_a[m_a.len-1]
if flags&1
	SetWindowState h 6 1
else
	SetWindowState h 7 1
	SetForegroundWindow hwnd ;;caller may call setcapture etc

 normally minimizing root owner hides all owned windows, but in some cases doesn't. On restore always shows.
for(_i 0 m_a.len-1) hid m_a[_i]; err
