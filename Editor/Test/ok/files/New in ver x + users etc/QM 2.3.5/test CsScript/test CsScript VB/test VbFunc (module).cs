out VbFunc("" 5 2)
#ret
Imports System
Module Test 'private
Function Add(a as Integer, b as Integer) As Integer
 Return a+b
End Function
End Module

Public Module Test2
Function Add2(a as Integer, b as Integer) As Integer
 Return a+b+1000
End Function
End Module
