out

 ExcelSheet es.Init("" 16)
ExcelSheet es.Init
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

 es.AddCode(code "mmmo")
 es.AddCode(code "ThisWorkbook")
es.AddCode("" "ThisWorkbook" 2)
 es.RunMacro("TestSetCell")
 out es.RunMacro("TestMsgBox" 100)
