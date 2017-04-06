str s sn; int i
ARRAY(int) ai.create(a.len)

for(i 0 a.len)
	int h=a[i]
	if(!IsWindow(h)) continue
	
	sn.getwintext(h)
	if(sn.len>200) sn.fix(197); sn+"..."
	sn.escape(3)
	sn.findreplace(":" "[91]58]")
	
	ai[i]=GetWindowIcon(h)
	
	s.formata("%s :MTB_Unhide %i * %i[]" sn h ai[i])

DynamicMenu s "temp_taskbar_menu" 1

for(i 0 ai.len) DestroyIcon(ai[i])
