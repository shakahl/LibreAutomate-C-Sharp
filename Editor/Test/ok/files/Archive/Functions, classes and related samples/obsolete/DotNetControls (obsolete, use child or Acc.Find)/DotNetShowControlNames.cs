 /
function hwndParent

 Displays names of all controls in .NET window.
 Does not throw errors.

 EXAMPLE
 out
 int w1=win("Form1")
 DotNetShowControlNames w1


#compile __DotNetControls
DotNetControls c.Init(hwndParent)

ARRAY(int) a; int i
child "" "" hwndParent 0 0 0 a
for i 0 a.len
	int h=a[i]
	str sn sc.getwinclass(h) st.getwintext(h)
	if(!c.GetControlName(h sn)) sn="???"
	out "name=''%s''  class=''%s''  text=''%s''" sn sc st

err+ out _error.description
