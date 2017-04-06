str s=
 johndoe@gmail.com,johndoe,doepassword
 jackjohnson@hotmail.com,jackj87,mypass123
 anony.mouse@gmail.com,anonmouse,cheesepw
 wowguy61@hotmail.com,wowguy61,wowrulesspassword

ARRAY(str) a=s
int i
for i 0 a.len
	str email user password
	if(tok(a[i] &email 3 ",")<3) end "bad format"
	email.trim; user.trim; password.trim
	out F"email=''{email}'',  user=''{user}'',  password=''{password}'"
	 here
	 add
	 code
	 that
	 uses
	 the
	 data
