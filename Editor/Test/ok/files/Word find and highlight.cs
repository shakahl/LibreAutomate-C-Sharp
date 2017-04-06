 /exe 1
out

typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Application app._getactive ;;connect to Word. Note: need the /exe 1.
Word.Document doc=app.ActiveDocument
str s=doc.Content.Text
 out s
s.findreplace("[13]" "[10]") ;;Word text newlines are [13]
str rx=
 (?xm)
 \n\n/.+
 |<(help|color|tip|code)
 \b.*?>(?s).*?</\1>
ARRAY(POINT) a; int i
if(!findrx(s rx 0 4 a)) end "failed, probably incorrect regular expression"
 out a.len
for i 0 a.len
	VARIANT v1(a[0 i].x) v2(a[0 i].y)
	doc.Range(v1 v2).HighlightColorIndex=Word.wdYellow

 BEGIN PROJECT
 main_function  Word find and highlight
 exe_file  $my qm$\Word find and highlight.qmm
 flags  6
 guid  {9C4658F9-BB16-4496-86C2-402388738F60}
 END PROJECT
