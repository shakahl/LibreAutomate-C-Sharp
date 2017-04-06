function! $map $icons flags

if(AddIcons2(map icons flags))
	m_pen=CreatePen(PS_SOLID 1 0xffa080)
	ret 1
else
	Delete
