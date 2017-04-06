 /
function'UIA.IUIAutomationElement UIA.CUIAutomation'u UIA.IUIAutomationElement'parent controlType $name flags ;;flags: 1 wildcard

UIA.IUIAutomationCondition pc pc1 pc2
if flags&1=0
	if !empty(name)
		if controlType
			pc1=u.CreatePropertyCondition(UIA.UIA_ControlTypePropertyId controlType)
			pc2=u.CreatePropertyCondition(UIA.UIA_NamePropertyId name)
			pc=u.CreateAndCondition(pc1 pc2)
		else
			pc=u.CreatePropertyCondition(UIA.UIA_NamePropertyId name)
	else
		pc=u.CreatePropertyCondition(UIA.UIA_ControlTypePropertyId controlType)
	ret parent.FindFirst(UIA.TreeScope_Subtree pc)

if(controlType) pc=u.CreatePropertyCondition(UIA.UIA_ControlTypePropertyId controlType)
else pc=u.CreateTrueCondition()

UIA.IUIAutomationElementArray a=parent.FindAll(UIA.TreeScope_Subtree pc)
int i n=a.Length
 PN
 out n
for i 0 n
	UIA.IUIAutomationElement ef=a.GetElement(i)
	str s=ef.CurrentName
	if(matchw(s name))
		 out i
		ret ef
