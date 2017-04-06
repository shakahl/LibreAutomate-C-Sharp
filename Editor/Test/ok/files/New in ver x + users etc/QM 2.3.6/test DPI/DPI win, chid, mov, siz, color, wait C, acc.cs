out
 outw win("" "" "" 0 "xy=1200 500")
 outw child("" "" "SyncBack" 0 "xy=43 230")

 int w=win("Untitled - Notepad" "Notepad")
 int w=win("Microsoft Spy++ - [Windows 1]" "Afx:*" "" 0x4)

 mov 1000 500 w
 mov 1000 500 w 1
 mov 1000 500 w 2
 mov 0.5 0.5 w 4

 siz 300 200 w
 siz 300 200 w 1
 siz 300 200 w 2
 siz 0.5 0.5 w 4

 int c=id(15 w)
 int c=child("" "SysTreeView32" w 0x0 "id=59648") ;;outline
 siz 0.5 0.5 c
 mov 0.5 0.5 c

 int w=win("SyncBack" "TfrmMain")
 int color=pixel(806 324 w 1)
 if(color=0x33cb67)
	 out F"0x{color}"
 outx wait(0 C 0x33cb67 806 324 w 1)

 int w1=win("SyncBack" "TfrmMain")
  Acc a.Find(w1 "CLIENT" "" "class=TPanel[]xy=0 294" 0x1004)
 
  Acc a=acc(0 294 w1 1)
 Acc a=acc(10 421 w1)
 int x y; a.Location(x y); mou x y

