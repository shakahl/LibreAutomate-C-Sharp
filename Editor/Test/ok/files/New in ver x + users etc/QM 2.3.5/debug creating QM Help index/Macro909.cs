int w1=_hwndqm
Acc a.Find(w1 "PUSHBUTTON" "Help Search history and options" "class=ToolbarWindow32[]id=2054" 0x1005)
rep 1
	a.DoDefaultAction
	lef 78 478 wait(5 WV win("" "#32768")) 1 ;;menu item 'Options...'
	int w2=wait(5 win("Help Search Options" "#32770"))
	lef 45 251 w2 1 ;;push button 'OK'
	int w3=wait(3 WV win("osd_indexer" "QM_OSD_Class"))
	wait 0 -WC w3
	0.5
