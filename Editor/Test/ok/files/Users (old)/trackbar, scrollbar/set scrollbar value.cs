Acc a=acc("Line down" "PUSHBUTTON" win("" "ExploreWClass") "SysListView32" "" 0x1021)
rep 3
	a.DoDefaultAction

 IE
 Htm el=htm("BODY" "" "" win("" "IEFrame") 0 0 0x20)
 el.Scroll("down")
