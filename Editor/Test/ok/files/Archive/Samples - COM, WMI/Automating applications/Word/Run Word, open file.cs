 declare Word type library. To insert this code, you can use 'Type libraries' dialog in Tools menu.
typelib Word {00020905-0000-0000-C000-000000000046} 8.0
 start Word, make visible, get Documents interface
Word.Application app._create
app.Visible = TRUE
Word.Documents docs=app.Documents
 load file
VARIANT d=_s.expandpath("$desktop$\test.doc")
Word.Document doc=docs.Add(d)
3
 exit
app.Quit
