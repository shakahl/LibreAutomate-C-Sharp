out
 lpstr macro=+qmitem
lpstr macro=+-1
 lpstr macro="Toolbar1"
str s
s="AAAAAAAAAAA"
str resName="rt:test"
 str resName="rt:test2"

PF
_qmfile.ResourceAdd(macro resName s s.len)
 s="BB"; _qmfile.ResourceAdd(macro resName s s.len)
PN
str s1
rep(3) _qmfile.ResourceGet(macro resName s1); PN
PO
out s1

 _qmfile.ResourceDelete(macro resName)
 _qmfile.ResourceDelete(macro "*")

out "------"

ARRAY(str) an ad; int i
 _qmfile.ResourceEnum(macro "rt:*" an)
 _qmfile.ResourceEnum(macro "rt:*" an ad)
_qmfile.ResourceEnum(macro "*" an ad)
for i 0 an.len
	out an[i]
	out ad[i]
