str username="Gintaras"
str password="********"

str s
Http h.Connect("www.quickmacros.com")

 get sid because it is different everytime
h.FileGet("forum/ucp.php?mode=login" s)
str sid
if(findrx(s "name=''sid'' value=''(.+?)''" 0 0 sid 1)<0)
	goto g1
	ret
out sid

 post form
ARRAY(POSTFIELD) a.create(4)
a[0].name="username"; a[0].value=username
a[1].name="password"; a[1].value=password
a[2].name="sid"; a[2].value=sid
a[3].name="login"; a[3].value="Login"
h.PostFormData("forum/ucp.php?mode=login" a s)

 see what we have
h.FileGet("forum\index.php" s)
 out s
 g1
str f="$temp$\qm forum.htm"
s.setfile(f)
run f
