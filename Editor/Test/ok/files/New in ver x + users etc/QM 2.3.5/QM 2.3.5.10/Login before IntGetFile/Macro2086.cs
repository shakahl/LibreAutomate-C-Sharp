out
str sHtml sHeaders
Http h.Connect("quickmacros.com")
 h.Get2("/forum/ucp.php?mode=login" _s 0 INTERNET_FLAG_NO_COOKIES sHeaders)
 out sHeaders; ret
h.Get2("/forum/ucp.php?mode=login" _s 0 INTERNET_FLAG_NO_COOKIES sHeaders)
out sHtml
out "-------------------"
out sHeaders
ret

h.PostAdd("username" "Gintaras")
h.PostAdd("password" "*")
h.PostAdd("autologin" "")
h.PostAdd("viewonline" "")
h.PostAdd("redirect" "index.php")
h.PostAdd("sid" "0f2cd72a3414b26866ec43f18a968eb1")
h.PostFormData("/forum/ucp.php?mode=login" 0 sHtml "" 0 0 0 sHeaders)
 out sHtml
out "-------------------"
out sHeaders
 ARRAY(str) a
 if(findrx(sHeaders "(?m)^Set-Cookie: (.+); " 0 4 a)) end "'Set-Cookie' header no found"
