out

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
 es.AddCode("" "" 2) ;;clears the module
