out
str s rh
int usefile=0

Http h.Connect("www.quickmacros.com")
h.SetProgressDialog(1 "custom title")
h.SetProgressCallback(&wininet_cb 8)

 h.Post("test/test.php" "a=b" s "" 0 rh)
 out s.len
 out rh

s="$temp$\test.png"
rep 2
	h.Post("test/test.php" "a=b" s "" 0 rh usefile)
 run s
out rh
