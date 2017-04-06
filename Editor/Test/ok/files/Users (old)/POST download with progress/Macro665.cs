out
Http h.Connect("www.quickmacros.com")
str s
ARRAY(POSTFIELD) a
if(!h.PostFormData2("test/test.php" a s "" 0 0 0 0 1)) end "failed"
str f.expandpath("$desktop$\qm.png")
s.setfile(f)
 run f
