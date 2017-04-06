out
int h=id(2205 _hwndqm)
if(hid(h)) ret

int i n
LVITEM li
str s ss.all(300) sss
ARRAY(STRINT) a

 get item text and image index
n=SendMessage(h LVM_GETITEMCOUNT 0 0)
a.create(n)
for i 0 n
	li.iItem=i
	li.mask=LVIF_TEXT|LVIF_IMAGE
	li.pszText=ss; li.cchTextMax=300
	if(!SendMessage(h LVM_GETITEM 0 &li)) end "error"
	a[i].i=li.iImage; a[i].s=li.pszText

 get category or class name (last line, first word)
SciGetText GetQmCodeEditor sss
sss.getl(sss numlines(sss)-1)
sss.gett(sss 0)

 format
s.from("<td>" sss "[]<ul>")
for i 0 n
	s.formata("<li id=''i%i''>%s</li>" a[i].i a[i].s)
if(n<a.len) s.formata("<li id=''i14''>... (%i items)</li>" a.len)
s+"</ul></td>[][]"

s.setclip
out "Stored to clipboard:"
out s

#if 0
