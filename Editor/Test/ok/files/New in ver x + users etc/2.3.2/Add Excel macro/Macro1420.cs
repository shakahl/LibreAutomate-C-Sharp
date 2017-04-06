 /exe 1

 This version uses typelib for VBProject etc.

ExcelSheet es.Init
Excel.Workbook wb=es.ws.Application.ActiveWorkbook

typelib VBIDE {0002E157-0000-0000-C000-000000000046} 5.3

 VBIDE.VBProject vbp=+wb.VBProject
 VBIDE.VBComponents vbc=vbp.VBComponents
IDispatch objWorkbook=wb
VBIDE.VBComponents vbc = objWorkbook.VBProject.VBComponents
VBIDE.VBComponent xlmodule = vbc.Add(1)
err mes- "Failed. You probably need to change Excel security settings.[][]Click menu Tools -> Macro -> Security.[]In Publishers tab check 'Trust access to Visual Basic project'." "" "x"

str strCode =
 sub test()
  msgbox "Inside the macro"
 end sub
 
xlmodule.CodeModule.AddFromString(strCode)
