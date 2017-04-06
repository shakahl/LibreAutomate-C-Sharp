 /
function# $accname $accrole $accclass accflags $wintext [$winclass] [$exename] [winflags] [Acc&ao]

 Finds window that contains specified accessible object (AO).
 Returns window handle. If not found, returns 0.

 First 4 parameters are same as with function <help>acc</help>.
 Next 4 parameters - as with function <help>win</help>.
 ao - variable that receives the accessible object.

 REMARKS
 Finding AO is slow. To make the function faster, use as many as possible properties.
 For example, if you don't use window properties, the function will search in all windows, which can take several minutes in some cases.
 If possible, instead use <help>win</help>. It can find window that contains specified control.

 Added in: QM 2.3.0.

 EXAMPLE
  Find window that has button "Hex":
 int h=WinA("Hex" "RADIOBUTTON" "Button" 0 "Calculator")


ARRAY(int) ah; int i
win(wintext winclass exename winflags 0 0 ah)
for i 0 ah.len
	Acc a=acc(accname accrole ah[i] accclass "" accflags); err continue
	if(a.a)
		if(&ao) ao=a
		ret ah[i]
