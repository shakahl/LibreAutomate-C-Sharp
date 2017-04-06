str s
DATE d dNow.getclock

 download Xmarks page
str cache="$temp qm$\xmarks.htm"; Dir f
if !f.dir(cache)
	if(!sub.Update) ret
else if dNow-f.TimeModified2>1.0
	mac "sub.Update"
s.getfile(cache)
 out s

ARRAY(str) a
findrx(s "<a href=''(https?://www.youtube.com/watch.+?)''" 0 4 a)
 out a.len
 for(_i 0 a.len) out a[1 _i]
 ret
 g2
s=a[1 RandomInt(0 a.len-1)]

 don't play a video more than 1 time in 2 days
str crc.from(Crc32(s s.len))
if(rget(d crc "\Xmarks") and dNow-d<2) goto g2
rset dNow crc "\Xmarks"

act "Firefox"
err
	run s
	ret
 key Ad ;;sometimes does not work
 ----
int w=win("Firefox" "Mozilla*WindowClass" "" 0x804)
rep 5
	Acc a1.Find(w "COMBOBOX" "Search or enter address" "class=MozillaWindowClass" 0x1005)
	err 0.1; continue ;;sometimes "object not found"
	break
if(!a1.a) OnScreenDisplay "failed to get address bar acc"; ret
a1.Select(3)
0.1
key E CSH ;;Select() sometimes appends instead of replacing
s.setsel; err ret
0.1
key Y


#sub Update
str s cache="$temp qm$\xmarks.htm"
int downlFailed
OnScreenDisplay "Xmarks - updating" 1
IntGetFile "http://share.xmarks.com/folder/bookmarks/JQk2OrPIPd" s
err downlFailed=1
if(s.len<10000) downlFailed=1 ;;an error page
if(downlFailed) OnScreenDisplay "Xmarks updating failed"; ret
s.setfile(cache)
ret 1
