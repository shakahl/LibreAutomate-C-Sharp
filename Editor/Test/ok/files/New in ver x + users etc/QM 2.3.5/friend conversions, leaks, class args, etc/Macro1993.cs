 ExcelSheet es.Init
 es.ws.Range("A1").Value=VariantBool(1)
 if(es.ws.Range("A1").Value) out "True"; else out "False"

str code=
 public function Test(byval b)
 if b=True then MsgBox("yes")
 MsgBox(b)
 end function
VbsAddCode(code)
 VbsFunc("Test" 1)
VbsFunc("Test" VariantBool(1))
