 To click something without the mouse, sometimes can be used WM_LBUTTONDOWN message. Try it if other methods don't work.
 This macro clicks an accessible object (report-style listview column header) in a folder.

int folderwindow=win("mp3" "CabinetWClass")
Acc a=acc("Album" "COLUMNHEADER" folderwindow "SysHeader32" "" 0x1001)
int x y w h
a.Location(x y w h)
int b=child(a)
ScreenToClient b +&x
x+w/2; y+h/2 ;;center
int xy=(x&0xffff)|(y<<16)
SendMessage(b WM_LBUTTONDOWN 1 xy)
SendMessage(b WM_LBUTTONUP 0 xy)
