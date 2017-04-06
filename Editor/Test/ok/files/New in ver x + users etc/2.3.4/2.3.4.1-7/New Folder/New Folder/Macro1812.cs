str s=
 johndoe@gmail.com,johndoe,doepassword
 jackjohnson@hotmail.com,jackj87,mypass123
 anony.mouse@gmail.com,anonmouse,cheesepw
 wowguy61@hotmail.com,wowguy61,wowrulesspassword

ICsv x=CreateCsv(1)
x.FromString(s) ;;or use FromFile("file path") to load directly from file
int i
for i 0 x.RowCount
	str email(x.Cell(i 0)) user(x.Cell(i 1)) password(x.Cell(i 2))
	out F"email=''{email}'',  user=''{user}'',  password=''{password}''"
	 here
	 add
	 code
	 that
	 uses
	 the
	 data
