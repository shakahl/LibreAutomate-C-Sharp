int w1=win("Mind map - Wikipedia, the free encyclopedia - Windows Internet Explorer" "IEFrame")
act w1
Htm el=htm("A" "Mental structures" "" w1 0 167 0x21)
int x y
el.Location(x y)
 el.Location(x y 0 0 child("" "Internet Explorer_Server" w1))
mou x y
