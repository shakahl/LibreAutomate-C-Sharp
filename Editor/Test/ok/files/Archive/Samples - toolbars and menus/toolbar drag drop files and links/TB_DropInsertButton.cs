 /tb_drag_drop3
function hWnd lParam

 Call this function on WM_QM_DRAGDROP in toolbar hook function.
 Inserts button(s).

 hWnd, lParam - hWnd, lParam.

 EXAMPLE
 sel message
, case WM_INITDIALOG
, int htb=wParam
, QmRegisterDropTarget(htb hWnd 16)
, 
, case WM_DESTROY
, case WM_QM_DRAGDROP
, TB_DropInsertButton hWnd lParam


QMDRAGDROPINFO& di=+lParam
str s1 s2 sn st si; int i b

b=TB_GetDragDropButton(hWnd)

foreach(s1 di.files)
	if(!s1.beg("::")) s2.getfilename(s1 1)
	if(!inp(s2 "Button label." "" s2)) ret
	s1.expandpath(s1 2)
	s1.escape(1); s2.escape(1)
	si.formata("%s :run ''%s''[]" s2 s1)

sn.getwintext(hWnd)
st.getmacro(sn)
if(b) i=findl(st b); else i=-1
if(i<0) i=st.len
st.insert(si i)
st.setmacro(sn)

err+ out _error.description
