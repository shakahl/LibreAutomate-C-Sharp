str s
 Acc a=acc("Mouse" "PUSHBUTTON" "QM TOOLBAR" "ToolbarWindow32" "" 0x1)
 a.DoDefaultAction

 Acc a=acc("men" "OUTLINEITEM" "Quick Macros" "SysTreeView32" "" 0x1)
 a.Select(SELFLAG_TAKEFOCUS|SELFLAG_TAKESELECTION)

 mac "DialogAccSelect"
 Acc a=acc("three" "LISTITEM" "Form" "ListBox" "" 0x1)
 a.Select(SELFLAG_TAKESELECTION)
 a.Select(SELFLAG_ADDSELECTION)
 a.Select(SELFLAG_REMOVESELECTION)

 int x y cx cy
 a.Location(x y cx cy)
 out cx

 s=a.Name
 out s
 s=a.Value
 out s
 s=a.Description
 out s

 Acc a=acc("" "TEXT" "Quick Macros" "Edit" "" 0x800 0x40 0x20000040)
 act id(2200)
 a.SetValue("!Notepad")

 str role1 role2
 int r=a.Role(role1 role2)
 out "%i %s %s" r role1 role2

 str state1
 int f=a.State(state1)
 out "0x%X %s" f state1

 a.Focus()
 'TT
 Acc a=acc("" "LIST" "Form" "ListBox")
 a.Focus(1)
 out a.Name

 Acc a=acc("" "LIST" "Form" "ListBox")
 ARRAY(Acc) se
 a.Selection(se)
 for(_i 0 se.len) out se[_i].Name

 Acc a=acc("DialogAccSelect" "OUTLINEITEM" "Quick Macros" "SysTreeView32" "" 0x1)
 a.ObjectFromPoint(10 -5)
 a.ObjectFromPoint(150 337 0x2)
 out a.Name

 Acc p
 a.Navigate("pa" p)
 p.ObjectFromPoint(20 20 0x1)
 out p.Name

 a.Mouse(1)
 a.Mouse(2)
 a.Mouse(0 100)

