 In firefox, last.fm loved list, place mouse
 over an artist link (not over song link),
 and run this macro.
 It gets artist and song, and pastes in soulseek.

 get author and name
Acc ab as
ab=acc(mouse)
ab.Navigate("pa n n" as)
str s sb(ab.Name) ss(as.Name)
s.from(sb " " ss)
 out s

 search in SoulSeek
int h=win
int w1=act("SoulSeek ")
Acc a=acc("Search Files" "PAGETAB" w1 "SysTabControl32" "" 0x1001)
a.DoDefaultAction; 0.5
s.setsel
key Y

 adjust SoulSeek listview control (disabled because now another function does it)
 spe 10
 lef+ 149 8 child("" "SysHeader32" w1)
 lef- 436 10 child("" "SysHeader32" w1)
 lef 748 11 child("" "SysHeader32" w1)

 mou
act h

err+
