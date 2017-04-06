out
Http h.Connect("www.quickmacros.com")
ARRAY(POSTFIELD) a.create(2)
a[0].isfile=1; a[0].name="tfile"; a[0].value="$qm$\gdiplus_other.txt"
a[1].name="etc"; a[1].value="5555"
str s
h.PostFormData("test/post.php" a s)
out s
