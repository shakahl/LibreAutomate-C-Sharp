str R=VbFunc("" "test" 5)
out R

#ret
 'VB.NET code
Imports System
Public Class Test
Public Shared Function TestFunction(s As String, i As Integer) As Integer
	Console.Write(s): Console.Write(i) 'display in QM output
	Return i*2
End Function

 'add private functions here if need
End Class
