 this recorded code clicks the calendar icon to show the calendar popup window, for testing. Remove it and use your code to show it. Let variable w2 be its handle.
 ---- Recorded 2013.09.08 09:00:54 ----
opt slowkeys 1; opt slowmouse 1; spe 100
int w1=act(win("Handover - Mozilla Firefox" "Mozilla*WindowClass" "" 0x4))
lef 412 350 w1 1 ;;graphic
int w2=wait(15 win("Mozilla Firefox" "MozillaDialogClass"))
opt slowkeys 0; opt slowmouse 0; spe -1
 --------------------------------------

 get selected date
0.5
Acc a.Find(w2 "LINK" "" "class=MozillaDialogClass[]a:class=*selectedDay*" 0x1004 10) ;;note: added 10 s waiting
str day url date
day=a.Name
out day
url=a.Value
out url
if(findrx(url "&date=(\d\d/\d\d/\d\d\d\d)" 0 0 date 1)<0) end "date not found in the URL"
out date
