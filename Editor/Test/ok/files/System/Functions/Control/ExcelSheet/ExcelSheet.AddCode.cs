function [$code] [$module] [flags] ;;1 insert, 2 clear

 Adds, replaces or deletes a code module in VBAProject of this workbook.
 Then you can use the code with <help>ExcelSheet.RunMacro</help>.

 code - VBA code.
   If omitted or "", deletes the module. If flag 2, just clears its code.
   Else if the module exists, replaces its code. If flag 1, inserts.
   Else adds new module.
 module - module name. Default: "" - "QuickMacros".

 REMARKS
 By default this function fails, because of Excel security settings. To enable:
   Older Excel versions: menu Tools -> Macro -> Security.
   Newer Excel versions: File -> Options -> Trust -> Macro.
   Check 'Trust access to Visual Basic project'.

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init("" 16)
 str code=
  Sub TestSetCell()
  Range("A5").Select
  ActiveCell.FormulaR1C1 = "test"
  End Sub
 ;
  Function TestMsgBox(ByVal x)
  TestMsgBox=MsgBox(x, vbOKCancel)
  End Function
 ;
 es.AddCode(code) ;;creates module "QuickMacros" and ads the code
 es.RunMacro("TestSetCell")
 out es.RunMacro("TestMsgBox" 100)
 es.AddCode ;;deletes the module


WS

if(empty(module)) module="QuickMacros"
__ExcelState _.Init(this 1)

 typelib VBIDE {0002E157-0000-0000-C000-000000000046} 5.3
 Excel.Workbook b
 VBIDE.VBProject p
 VBIDE.VBComponents cc
 VBIDE.VBComponent c
 VBIDE.CodeModule m

 don't use typelib to avoid various possible problems
IDispatch b
IDispatch p
IDispatch cc
IDispatch c
IDispatch m

b=ws.Parent
p=b.VBProject
cc=p.VBComponents
c=cc.Item(module); err

if empty(code)
	if c
		if flags&2
			m=c.CodeModule
			m.DeleteLines(1 m.CountOfLines)
		else
			cc.Remove(c)
	ret

if c
	m=c.CodeModule
	if(flags&1=0) m.DeleteLines(1 m.CountOfLines)
else
	c=cc.Add(1) ;;VBIDE.vbext_ct_StdModule
	c.Name=module
	m=c.CodeModule

m.AddFromString(code)

err+ end _error
