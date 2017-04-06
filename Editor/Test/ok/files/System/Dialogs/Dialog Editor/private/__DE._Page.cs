function page ;;page: -2 on EN_CHANGE.

str s
if(page>-2)
	if(page>300) ret
	if(page>=0) s=page
	s.setwintext(id(1013 _hpane))
else
	s.getwintext(id(1013 _hpane))
	if(s.len)
		page=val(s)
		if(page<0 or page>300) SetDlgItemText(_hpane 1013 ""); ret
	else page=-1
	
	_page=page
	DT_Page _hform _page _pageMap
	
	EnableWindow(id(1011 _hpane) _page>=0)
	
	_Select(_hform)
	_save=1
