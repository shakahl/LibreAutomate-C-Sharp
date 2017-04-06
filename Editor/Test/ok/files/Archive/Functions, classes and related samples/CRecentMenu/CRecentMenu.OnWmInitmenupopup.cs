function# wParam

 Call this function on WM_INITMENUPOPUP.
 Populates "Recent" submenu.
 Returns number of files.

 wParam - wParam.


if(wParam!=m_hsubmenu) ret

int i n
for(i GetMenuItemCount(wParam)-1 -1 -1) DeleteMenu(wParam i MF_BYPOSITION)

str s s2
if(!rget(s "Recent" m_rkey)) ret

i=m_first_id
foreach s2 s
	AppendMenuW wParam 0 i @s2
	i+1
	n+1

ret n
