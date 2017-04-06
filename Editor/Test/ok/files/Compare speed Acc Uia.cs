 int w=win("Dialog" "#32770")
int w=win("FileZilla" "wxWindowNR")
int c=id(3 w) ;;push button 'Button'
 c=id(5 w) ;;editable text 'Text'

ARRAY(int) k; int i
child "" "" w 0 "" k
out k.len ;;20 in Dialog, 61 in FileZilla
 for(i 0 k.len) out _s.getwinclass(k[i])

int useAcc=0
PF
for i 0 k.len
	c=k[i]
	if useAcc
		Acc a.FromWindow(c)
		_s=a.Name
	else
		UIA.IUIAutomationElement e=Uia(c)
		_s=e.CurrentName
PN;PO

out _s

 speed with n controls in Dialog:
 acc: n=1 600,  n=20 3100
 uia: n=1 2000, n=20 21000

 speed with 61 controls in FileZilla:
 acc: 30000
 uia: 70000
