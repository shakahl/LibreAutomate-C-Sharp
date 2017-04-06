 this works, but cannot be used because requires UTF-16 string


#compile markup_services_decl

 str s="<HTML><BODY>Hello World!</BODY></HTML>"
str s.getfile("$desktop$\html.txt")

MSHTML.IHTMLDocument2 d=d._create(uuidof(MSHTML.HTMLDocument))
IPersistStreamInit ps=+d; ps.InitNew; ps=0

IMarkupServices ms=+d
IMarkupContainer mc
IMarkupPointer mkstart mkfinish
ms.CreateMarkupPointer(mkstart)
ms.CreateMarkupPointer(mkfinish)

ms.ParseString(+s.unicode 0 mc mkstart mkfinish)

MSHTML.IHTMLDocument2 d2=+mc

out d2.body.outerHTML
