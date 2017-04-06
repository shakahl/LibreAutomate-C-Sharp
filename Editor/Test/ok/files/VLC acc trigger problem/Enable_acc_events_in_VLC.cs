int w=TriggerWindow
Acc a.Find(w "PUSHBUTTON" "" "class=QWidget[]descr=^(Play|Pause)" 0x1088)
 out "VLC acc events enabled"

 Possibly enables events only of part of objects, including the Play/Pause button.
 If need to enable for all objects, use this slower code:
 Acc a.Find(w "ALERT")
