 get QM editor text
str s.getmacro; if(!s.len) ret

 find all '#sub Name' and ' $BkM optional name' lines
ARRAY(CHARRANGE) a; int i
if(0=findrx(s "(?m)^(?:#sub +(\w+)| \$BkM\b([^\r\n]*))" 0 4 a)) ret

 format items text for ListDialog
str items t
for i 0 a.len
	int isBM=s[a[0 i].cpMin]!='#'
	CHARRANGE& r=a[1+isBM i]
	int n=r.cpMax-r.cpMin; if(n>0) t.get(s r.cpMin n); t.trim; else t=""
	items.formata("%i %s[]" isBM t)

i=ListDialog(items "Subfunctions and code bookmarks" "Go to" 128|4 0 0 0 0 0 "$qm$\function.ico[]$qm$\favorites.ico")-1
if(i<0) ret
int c=GetQmCodeEditor
SendMessage(c SCI.SCI_GOTOPOS a[0 i].cpMin 0)
