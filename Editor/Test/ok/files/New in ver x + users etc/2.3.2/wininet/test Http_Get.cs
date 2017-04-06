out
str s rh
int usefile=16

Http h.Connect("www.quickmacros.com")
h.SetProgressDialog(1)
h.SetProgressCallback(&wininet_cb 7)

 rep 5
	 out h.Get("test/test.txt" s usefile 0 rh)
 out h.FileGet("test/test.txt" s usefile 0 0 0 rh)

s="$temp$\test.txt"
Q &q
 out h.Get("test/test.txt" s usefile 0 rh)
out h.Get("quickmac.exe" s usefile|0 0 rh)
 out h.lasterror
Q &qq; outq
 run s

out s.len
out rh
