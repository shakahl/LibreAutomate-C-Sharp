out
Http h

 h.GetUrl("http://www.quickmacros.com" _s)
 out _s

h.Connect("www.quickmacros.com")
 h.FileGet("support.html" _s)
 out _s

ARRAY(POSTFIELD) a.create(2)
a[0].name="txt"; a[0].value="some text"; a[0].isfile=0
a[1].name="userfile"; a[1].value="$desktop$\test.txt"; a[1].isfile=1
str r
if(!h.PostFormData("form.php" a r)) end "failed"
out r
