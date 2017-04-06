 Edit and run this function.
 To edit when already running, end "tooltip1" thread.

SetThreadPriority GetCurrentThread THREAD_PRIORITY_LOWEST
POINT p pp
rep
	0.5
	xm p
	if(!memcmp(&p &pp 8)) continue
	pp=p
	 out "mouse moved"
	
	int ttId=0; str ttText
	 ----------- BEGIN EDIT --------------
	 Look where is mouse.
	 To show tooltip, set ttId (a unique nonzero number) and ttText.
	 Examples:
	
	int w=win(mouse)
	Acc a=acc(mouse)
	if wintest(w "Quick Macros" "QM_Editor") and acctest(a "global_tooltip" "OUTLINEITEM" w "id=2202 SysTreeView32")
		ttId=1
		ttText="'global_tooltip' in QM list of macros"
	else if wintest(w "Notepad" "Notepad") and acctest(a "Edit" "MENUITEM" w "Notepad" "" 0x1)
		ttId=2
		ttText="menu item 'Edit' in Notepad"
	 ...
	
	 ----------- END EDIT --------------
	
	int pttId
	if(ttId=pttId) continue
	if(pttId) shutdown -6 0 "ShowTooltip"
	pttId=ttId
	if(!ttId) continue
	 out ttText
	mac "ShowTooltip" "" ttText 30 p.x p.y ScreenWidth/2 1
	
