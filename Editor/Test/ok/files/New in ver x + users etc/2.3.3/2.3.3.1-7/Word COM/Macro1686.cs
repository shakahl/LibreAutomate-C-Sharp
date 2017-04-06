 /exe 1
typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Application a._getactive
VARIANT v1 v2
v1=Word.wdLine; v2=Word.wdExtend
a.Selection.HomeKey(v1)
a.Selection.EndKey(v1 v2)
a.Selection.Range.Case = Word.wdTitleWord

 BEGIN PROJECT
 main_function
 END PROJECT
