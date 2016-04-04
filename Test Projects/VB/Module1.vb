'Imports System.Runtime.CompilerServices
'Imports System.Windows.Forms
Imports K = System.Windows.Forms.Keys
'Imports System.Windows.Forms.Keys

Imports Catkeys
Imports Catkeys.NoClass
Imports Catkeys.Automation
Imports Catkeys.Automation.NoClass
Imports Catkeys.Automation.Input

Imports Catkeys.Show


Module Module1

	Sub Main()

		'Show.MessageDialog("ggg")
		'Show.TaskDialog("ddd")
		'TaskDialog("ddd") 'error, ambiguous if Imports Catkeys.Show, although OK in C#

		'Console.WriteLine("test")
		''Util.Tesst.Koo()
		''Util.Tesst.Koo()
		'Dim s As String = " ff"
		'Console.WriteLine(s.Trim)
		'Console.WriteLine(s.Trim) 'VS replaces trim with Trim
		''Console.WriteLine(s.trim2)

		'Dim w As Wnd = Wnd.Get.DirectParent(Wnd0)

		'Out(Keys.Control)
		'Out(Shift)
		'Out(K.Control)
		'Input.Keys("")
		'Keys("")
		'Key("")
		'Input.Key("")
	End Sub

End Module
