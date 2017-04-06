int w=win("Keyboard Layout Creator 1.4 - 'Layout01 Description'" "WindowsForms10.Window.8.app.0.378734a")
 Acc a.Find(w "PUSHBUTTON" "Caps" "class=*.BUTTON.*[]wfName=VK_CAPITAL" 0x1005)
 out a.a

 out
 ShowAccDescendants w

Acc b.FromWindow(w)
out b.a
out b.ChildCount
