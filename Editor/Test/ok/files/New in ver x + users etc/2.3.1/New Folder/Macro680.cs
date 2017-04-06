out
Http h.Connect("www.quickmacros.com")
str s
out h.PostWithTimeout(2 "form2.php" "a=1&b=2" s)
out s
