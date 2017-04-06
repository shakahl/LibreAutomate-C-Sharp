rep
	wait 0 K C
	int i=GetMenuItemIdFromMouse(_s _i)
	if(!i) continue
	out i
	 out _i
	out _s
	 MenuSetString(_i i "hhhhh")
	

 Other way would be to use acc. MenuItemFromPoint -> acc.elem.
