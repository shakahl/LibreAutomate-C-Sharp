 /
function# $childtext [$childclass] [childflags] [$wintext] [$winclass] [$exename] [winflags] [int&hwndchild]

 Finds window that contains specified control.
 Returns window handle. If not found, returns 0.

 First 3 parameters are same as with function <help>child</help>.
 Next 4 parameters - as with function <help>win</help>.
 hwndchild - variable that receives child window handle.

 REMARKS
 QM 2.3.4. <help>win</help> also supports it.

 EXAMPLE
  Find window that has button "Hex":
 int h=WinC("Hex" "Button" 0 "Calculator")


ARRAY(int) ah; int i
win(wintext winclass exename winflags 0 0 ah)
for i 0 ah.len
	int h=child(childtext childclass ah[i] childflags); err continue
	if(h)
		if(&hwndchild) hwndchild=h
		ret ah[i]
