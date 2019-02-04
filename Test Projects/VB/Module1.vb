'Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports System.Drawing
Imports K = System.Windows.Forms.Keys
'Imports System.Windows.Forms.Keys

Imports Au
Imports Au.NoClass
'Imports Au.Input
Imports Au.Triggers

Imports System.Runtime.InteropServices

Module Module1

    Sub Main()

        TestQm2SendMessage()

        'Triggers.Of.Window("moo")

        'Key("Tab")

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

        'Wnd.Find(, "cass")
        'Wnd.Find(className:="cass")

    End Sub

    'Imports System.Runtime.InteropServices

    <DllImport("user32.dll", EntryPoint:="SendMessageW", CharSet:=CharSet.Unicode)>
    Function SendMessageS(hWnd As IntPtr, Msg As Int32, wParam As UInt32, lParam As String) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindowW", CharSet:=CharSet.Unicode)>
    Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function

    Sub TestQm2SendMessage()
        Dim hwnd = FindWindow("QM_Editor", Nothing)
        If hwnd = IntPtr.Zero Then Return
        SendMessageS(hwnd, 12, 1, "Q ' M 'Macro295' C test VB")
    End Sub


End Module
