out
ARRAY(POSTFIELD) a.create(3)
a[0].name="username"; a[0].value="Gintaras"
a[1].name="password"; a[1].value="*"
 a[2].name="autologin"; a[2].value=""
 a[3].name="redirect"; a[3].value=""
 a[4].name="login"; a[4].value="Log in"
a[2].name="file"; a[2].value="$desktop$\test.txt"; a[2].isfile=1

Http h.Connect("www.quickmacros.com")
 Deb
str srh
h.PostFormData("forum/login.php" a _s "" 0 0 inet.INTERNET_FLAG_NO_AUTO_REDIRECT srh)
out srh
out _s
