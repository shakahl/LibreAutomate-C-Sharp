ExcelSheet es.Init

typelib VBIDE {0002E157-0000-0000-C000-000000000046} 5.3
Excel.Workbook b=es._Book
 IDispatch d=b.VBProject
 VBProject p=b.VBProject
VBIDE.VBProject p=b.VBProject
 error: type library error: type name not unique. VBProject.
 can use Excel.VBProject (gets from the same typelib referenced inside), but the no typeinfo

out p
