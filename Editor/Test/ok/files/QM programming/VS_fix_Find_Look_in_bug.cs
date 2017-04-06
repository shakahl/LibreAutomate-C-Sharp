 Workaround Visual Studio 2015 bug: often selects random item in "Look in" combobox in "Find and Replace" pane.
 This macro selects "Current Project" when the pane becomes focused.

function hwnd idObject idChild
int hcb=id(1107 hwnd)
0.1
 outw hcb

str sMustBe="Current Project"

str s.getwintext(hcb)
if(s=sMustBe or s="Entire Solution") ret

int i=CB_FindItem(hcb sMustBe 0 1)
 out i
if(i<0) ret
CB_SelectItem(hcb i)
