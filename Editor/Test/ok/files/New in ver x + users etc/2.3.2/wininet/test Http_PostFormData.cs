out
str s rh

Http h.Connect("www.quickmacros.com")
h.SetProgressDialog(1)
h.SetProgressCallback(&wininet_cb 9)

 ARRAY(POSTFIELD) a.create(2)
  a[0].name="a"; a[0].value="$desktop$\test.txt"
 a[0].name="c"; a[0].value="calc.exe"; a[0].isfile=1
 a[1].name="n"; a[1].value="notepad.exe"; a[1].isfile=1

h.PostAdd("c" "winapi7.txt" 1)
h.PostAdd("n" "notepad.exe" 1)

 if(!h.PostFormData("test/test.php" 0 s "" 0 0 0 rh)) end h.lasterror
if(!h.PostFormData("test/test.php" 0 s "" &test_PFD_cb 20 0 rh)) end h.lasterror

out s.len
out rh

 s="$temp$\test.png"; del- s; err
 h.PostFormData("test/test.php" a s "" 0 0 0 rh 16)
 run s
 out rh



