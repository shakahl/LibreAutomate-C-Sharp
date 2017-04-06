out
str f1.expandpath("$qm$\Catkeys\Help\HelpHtml")
str f2.expandpath("$qm$\Catkeys\Help\CatkeysHelp\Converted")
 del- f2; err ;;Windows bug: often does not show the new folder in the "browse folders" dialog
 if(FileExists(f2 1)) del- F"{f2}\*"; else mkdir f2; 1

Dir d
foreach(d F"{f1}\*.html" FE_Dir 4)
	str path=d.FullPath
	 out path
	sub.ConvertFile(path f1 f2)





#sub ConvertFile
function ~file ~f1 ~f2

IXml x1._create x2._create
x1.FromFile(file)

str aml=
 <?xml version="1.0" encoding="utf-8"?>
 <topic id="{TOPIC_ID}" revisionNumber="1">
 <developerConceptualDocument
 xmlns="http://ddue.schemas.microsoft.com/authoring/2003/5"
 xmlns:xlink="http://www.w3.org/1999/xlink">
 <introduction>
 <markup>{BODY}</markup>
 </introduction>
 </developerConceptualDocument>
 </topic>

aml.findreplace("{TOPIC_ID}" _s.getfilename(file) 4)

str html.getfile(file)
str head body title
if(findrx(html "(?s)<head>(.+?)</head>" 0 0 head 1)<0) end "no <head>" 1
if(findrx(html "(?s)<body>(.+)</body>" 0 0 body 1)<0) end "no <body>" 1
if(findrx(head "(?s)<title>(.+)</title>" 0 0 title 1)<0) end "no <title>" 1
ARRAY(str) ah; int i
if findrx(head "(?s)<(style|link|script)\b.+?</\1>" 0 4 ah)
	str t="[]    "
	for(i 0 ah.len) t+ah[0 i]; t+"[]"
	t+body
	t.swap(body)

aml.findreplace("{BODY}" body 4)

x2.FromString(aml) ;;just to ensure that it is valid XML

f2+(file+f1.len)
f2.replace(".aml" findcr(f2 '.'))
out f2
aml.setfile(f2)
