SHORTCUTINFO si
if(GetShortcutInfoEx("$desktop$\test.lnk" &si))
	out si.target
	out si.descr
	out si.hotkey
	out si.iconfile
	out si.iconindex
	out si.initdir
	out si.param
	out si.showstate
	
