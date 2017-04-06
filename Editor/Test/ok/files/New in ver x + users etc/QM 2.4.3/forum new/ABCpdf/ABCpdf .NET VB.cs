CsScript x.SetOptions("language=VB[]references=ABCpdf")
x.AddCode("")

int R=x.Call("Class1.TestFunction" _s.expandpath("$desktop$\QM ABCpdf VB.pdf"))
out R


#ret
'VB.NET code
Imports System
Imports WebSupergoo.ABCpdf9

Public Class Class1
Public Shared Function TestFunction(saveToThisFile As String) As Integer
Dim theDoc As New Doc
theDoc.FontSize = 96
theDoc.AddText("Quick Macros")
theDoc.Save(saveToThisFile)
theDoc.Clear()

Return 1
End Function
End Class
