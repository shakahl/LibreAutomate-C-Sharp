 Works only with Internet Explorer, and only in pages without frames.

 Excel sheet to array
ARRAY(str) a
ExcelToArray2 a "A:A"
 Web page to string
MSHTML.IHTMLElement el=htm("BODY" "" "" " Internet Explorer" 0 0 0x20)
str s=el.innerText
 Search for words
int i
for i 0 a.len
	if(find(s a[0 i] 0 1)>=0) break
if(i=a.len) ret ;;not found
 Use Find dialog
act "Internet Explorer"
key Cf
0.5
key (a[0 i]) Y
