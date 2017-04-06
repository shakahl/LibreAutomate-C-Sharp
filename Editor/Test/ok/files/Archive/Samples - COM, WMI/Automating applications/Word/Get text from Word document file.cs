typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Document d._getfile(_s.expandpath("$documents$\draugas.doc"))
Word.Range r

 get all text
r=d.Range
str sAllText=r.Text
out sAllText
