VbsAddCode ""
VbsFunc "Func1"
out VbsFunc("Func2" 5)

#ret
'VBScript code

Sub Func1
MsgBox "Func1"
End Sub

Function Func2(ByVal a)
MsgBox "Func2"
Func2=a*2
End Function
