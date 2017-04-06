 WHEN TO USE
 To quickly transfer macros, functions, etc from a QM forum post to QM.
 Use when they are embedded in the post, not attached as qm file.

 HOW TO USE
 In QM forum select whole or part of the post and copy to the clipboard (Ctrl+C).
 Then switch to QM and run this macro.
 This macro parses clipboard text, creates folder "from forum", and creates the macros/functions there.

 MORE INFO
 The post must contain one or more properly formatted QM code blocks (macros, functions, etc).
 You can copy only part of post. If there is function name or other info above code, include it. 
 This macro tries to extract QM item name, type and trigger from the one or two lines above the code.
 If there is no name, gives default name, eg "Macro23". On conflict renames.

 RECOMMENDED TRIGGER
 Add this macro to a menu whose trigger is QM events -> Add to a menu -> Menu bar.

 ___________________________________________________________________________________

 out

 get clipboard text in HTML format
str s s1 s2
s.getclip("HTML Format")
 out s
if(!s.len)
	 ge1
	mes- "The clipboard does not contain text copied from QM forum in HTML clipboard format." "" "x"

 get selected fragment
int i j
i=find(s "StartFragment:")
j=find(s "EndFragment:")
if(i<0 or j<0) goto ge1
i=val(s+i+14)
j=val(s+j+12)
if(!i or !j) goto ge1
s.get(s i j-i)
s-"<HTML><HEAD></HEAD><BODY>"; s+"</BODY></HTML>"
if(!_unicode) s.ConvertEncoding(CP_UTF8 0)
 out s


 parse HTML
HtmlDoc d.InitFromText(s)
 out d.GetHtml
 ret

type TNTI ~text ~name ~trigger ~itype
ARRAY(TNTI) a

 get text of code blocks
ARRAY(MSHTML.IHTMLElement) ae
d.GetHtmlElements(ae "div")
for i 0 ae.len
	MSHTML.IHTMLElement e=ae[i]
	s=e.className; if(s!="cod") continue
	s=e.innerText
	a[].text=s
	s-"<-<58321>->"; e.innerText=s ;;mark to find name and trigger later
	
if(!a.len) mes- "There is no code in the clipboard. In QM forum select text with one or more blocks of properly formatted QM code." "" "x"

 get name, item type and trigger
s=d.GetText
 out s
ARRAY(str) as
if(findrx(s "^((.+ \?[]){0,2})\s*<-<58321>->" 0 8|4 as)!=a.len) ret
for i 0 a.len
	TNTI& r=a[i]
	s=as[1 i]
	s1.getl(s 0)
	s2.getl(s 1)
	if(s1.len)
		s1.trim("?")
		sel s1 3
			case "Member Function *" r.itype="Member"; r.name.get(s1 16)
			case "T.S. Menu *" r.itype="T.S. Menu"; r.name.get(s1 10)
			case "TS Menu *" r.itype="T.S. Menu"; r.name.get(s1 8)
			case "Autotext *" r.itype="T.S. Menu"; r.name.get(s1 9)
			case else r.itype.gett(s1 0); r.name.gett(s1 1 "" 2)
		r.name.trim
	else r.name=""
	if(s2.len and s2.begi("Trigger "))
		r.trigger.get(s2 8 s2.len-10)
		r.trigger.trim
	 out r.name; out r.itype; out r.trigger; out r.text; out "------"

 correct text
for i 0 a.len
	&r=a[i]
	lpstr k=r.text
	rep
		rep() sel(k[0]) case ',' k[0]=9; k+1; case ';' k[0]=32; k+1; case else break
		k=strchr(k 10)+1; if(k=1) break

 create QM items
int ifolder=newitem("from forum" "" "Folder")
for i 0 a.len
	&r=a[i]
	newitem r.name r.text r.itype r.trigger +ifolder 16
