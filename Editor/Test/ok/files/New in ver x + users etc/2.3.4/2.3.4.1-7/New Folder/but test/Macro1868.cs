out
int w=win("Dialog" "#32770")
int c=id(8 w)
act w

 but c
 but% c
 but+ c
 but- c
 but* c

 rep 2
	 but c

 Acc a.Find(c "CHECKBUTTON" "" "" 0x1005)
 Acc a.Find(c "PUSHBUTTON" "" "" 0x1005)
 a.DoDefaultAction ;;BM_CLICK
