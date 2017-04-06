Http h.Connect("www.quickmacros.com")
str s
h.PostWithTimeout2(60 "form2.php" "a=1&b=2" s)
 out s
