AccClick("Bold" "PUSHBUTTON" "Macromedia Dreamweaver MX - [Whats new (QM_Help/IDH_WHATSNEW2.html)]" "Button" "" 0x1001)
AccSelect(SELFLAG_TAKEFOCUS "Inv" "CHECKBUTTON" "Calculator" "Button" "" 0x1001)
Acc a=acc("Degrees" "RADIOBUTTON" "Calculator" "Button" "" 0x1001)
int x y cx cy
a.Location(x y cx cy)
Acc a=acc("Bin" "RADIOBUTTON" "Calculator" "Button" "" 0x1001)
str v2=a.Name
str value=a.Value
Acc a=acc("Backspace" "PUSHBUTTON" "Calculator" "Button" "" 0x1001)
a.DoDefaultAction
Acc a=acc("Hyp" "CHECKBUTTON" "Calculator" "Button" "" 0x1001)
a.Select(SELFLAG_REMOVESELECTION)
a.SetValue("aser")
a.Mouse(1)
Acc v7_p=acc("Calculator" "PUSHBUTTON" "+Shell_TrayWnd" "ToolbarWindow32" "" 0x1001)
MSHTML.IHTMLElement v7_o=htm("TD" "MB_OKCANCEL" "" "MSDN Library - January 2002 - MessageBox" 0 12 0x21)
Acc 9v55=acc("Untitled - Notepad" "PUSHBUTTON" "+Shell_TrayWnd" "ToolbarWindow32" "" 0x1001)
v9v55.DoDefaultAction
Acc a=acc("Calculator" "PUSHBUTTON" "+Shell_TrayWnd" "ToolbarWindow32" "" 0x1001)
int v99=a.Role()
int v9=a.State()
a.Focus()
a.Focus(1)
ARRAY(Acc) as
a.Selection(as)
a.ObjectFromPoint(??? ???)
Acc v4___
a.Navigate("u p" v4___)
a.Mouse(4 9 9)
Acc a=acc("Calculator" "PUSHBUTTON" "+Shell_TrayWnd" "ToolbarWindow32" "" 0x1001)
if(!a.a) ret
