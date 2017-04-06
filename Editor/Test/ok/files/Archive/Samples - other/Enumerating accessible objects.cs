 Shows 3 ways to enumerate accessible objects that are direct children of a parent object.
 In newer QM versions you can instead use Acc.GetChildObjects.

out

Acc a
str s

 1. If children have nonzero elem (see in Find Acc. Obj. -> Properties -> Element), can enumerate using it.
 find parent object
a=acc("" "TOOLBAR" win("QM TOOLBAR" "QM_toolbar") "ToolbarWindow32" "" 0x1000)
 get number of children
int n=a.a.ChildCount
 enumerate by changing a.elem
for a.elem 1 n+1
	s=a.Name
	out s
out "---"

 2. The same can be used without knowing the number of children.
 find parent object
a=acc("" "TOOLBAR" win("QM TOOLBAR" "QM_toolbar") "ToolbarWindow32" "" 0x1000)
 enumerate by changing a.elem until error
for a.elem 1 1000000000
	s=a.Name; err break
	out s
out "---"

 3. If children's elem is 0, have to use Navigate.
 find parent object
a=acc("" "TOOLBAR" win("QM TOOLBAR" "QM_toolbar") "ToolbarWindow32" "" 0x1000)
 get first child (alternatively, acc could find first child instead of parent, and then this statement must not be used)
a.Navigate("first"); err ret
 enumerate by calling Navigate until error
rep
	s=a.Name
	out s
	a.Navigate("next"); err break
out "---"
