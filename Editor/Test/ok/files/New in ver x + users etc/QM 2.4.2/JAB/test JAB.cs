 atend eat_cpu 1; rep(4) mac "eat_cpu"
 0.5

out
int w=win("" "SunAwt*") ;;"SunAwtDialog", Stylepad, SwingSet2, jEdit, SampleTree, etc
 int w=win("OpenOffice" "SAL*FRAME")
 2; int w=win(mouse)
 outw w; ret
act w
 lef 240 300 w 1 ;; 'Global Options jEdit: General'
 lef
PF
Acc a
 a.Find(w "" "" "" 0 0 0 0 &sub.Callback_Acc_Find); ret
 a.Find(w "" "" "" 0x30 0 0 0 &sub.Callback_Acc_Find); ret
 a.Find(w "menu item" "Copy" "")
 a.Find(w "" "Bold" "" 0)
 a.Find(w "" "Font" "" 0)
 a.Find(w "text" "History text field entries to remember:" "" 1)
 a.Find(w "" "Street" "")
 a.Find(w "" "Draw" "")
 a.Find(w "" "Save" "")
 a.Find(w "combo box" "Save-" "")
a.Find(w "combo box" "" "")
 a.Find(w "" "editable" "")
 a.Find(w "radio button" "" "")
 a.Find(w "panel" "" "" 0 0 5)
 a.Find(w "" "" "value=200")
 a.Find(w "combo box" "Swing look & feel:" "" 0x1001)
 a.Find(w "combo box" "Apply Style" "" 0x1081)
 a.Find(w "label" "Load/Save" "" 1)
 a=acc(1337 95 0); mou 1337 95
 a=acc(xm ym)
 a=acc(xm ym 0)
 a.FromMouse
 a.FromXY(xm ym)
 a.FromFocus
 a.FromXYWindow(100 100 w 1)
 a.FromWindow(w)
 a.FromWindow(w OBJID_CLIENT)
PN
 shutdown -6 0 "eat_cpu"
if(!a.a) PN;PO; out "<not found>"; ret

out "----"
 outw child(a)
 a.Navigate("pa")
 a.Navigate("first")
 a.Navigate("last")
 a.Navigate("ne")
 a.Navigate("pr")
 a.Navigate("child2")
 a.Select(1)
 a.Select(2)
 a.Select(4) ;;extend (not impl)
 a.Select(8) ;;add
 a.Select(16) ;;remove
 a.SetValue("500")
 a.ObjectFromPoint(0 0 1) ;;not impl
 a.ObjectFocused(1) ;;not impl
 ARRAY(Acc) ar; a.ObjectSelected(ar) ;;not impl
 ARRAY(Acc) ar; a.GetChildObjects(ar); out ar.len
 ARRAY(Acc) ar; a.GetChildObjects(ar 0 "combo box" "o"); out ar.len
 out a.ChildCount
 a.CbSelect(1 1) ;;tested all cases
 outw a.Hwnd
 a.Mouse(1)

 out a.CompareProp("check boX" "Save*" "value=1[]xy=199 286" 1)
 out a.CompareProp("check boX" "Save*" "class=Sun*" 1)
 out acctest(a "" "" w)
 out acctest(a)
 out acctest(a w)
 out acctest(a mouse)
 POINT p; xm p w 1; out acctest(a p.x p.y w 1)
 POINT p; xm p w; out acctest(a p.x p.y)
 out acctest(a xm ym 0)
 ret
out "--"
a.Role(_s); out _s
a.State(_s); out _s
out a.Name
out a.Value
out a.a.DefaultAction
out "----"

PN;PO
a.showRECT

 a.DoDefaultAction
 a.JavaAction("togglePopup")
 PN;PO


#sub Callback_Acc_Find
function# Acc&a level cbParam

 Callback function for Acc.Find or acc.
 Read more about <help #IDP_ENUMWIN>callback functions</help>.

 a - the found object.
 level - its level in the hierarchy. If class or id is specified, it is level beginning from that child window. When searching in web page (flag 0x2000), it is level from the root object of the web page (DOCUMENT or PANE).
 cbParam - cbParam passed to Acc.Find, or y passed to acc.

 Return:
 0 - stop. Let a will be the found object.
 1 - continue.
 2 - continue, but skip children of a.


str s sr sa sn ss
a.Role(sr)
sn=a.Name; sn.LimitLen(15 1)
s.format("%-15s  %s" sr sn)
sr+"    ''"; sr+sn; sr+"''"
int test=2
sel test
	case 0
	out s
	
	case 1
	VARIANT v=1
	sa=a.a.DefaultAction(v); err
	 sa=a.a.DefaultAction; err
	 if(!sa.len) ret 1
	sa.findreplace("[]" "[][9][9][9][9][9][9][9][9][9][9][9][9]")
	out "actions:  %-37s %s" s sa
	
	case 2
	a.State(ss)
	 if(findw(ss "focused")<0) ret 1
	out "state:  %-37s %s" s ss
	

ret 1
