 /
function'UIA.IUIAutomationElement hwnd [$name] [flags] ;;flags: 1 case insensitive

UIA.CUIAutomation u._create
UIA.IUIAutomationElement ew ef
ew=u.ElementFromHandle(+hwnd); if(!ew) ret
if(empty(name)) ret ew

UIA.IUIAutomationCondition pc=u.CreatePropertyConditionEx(UIA.UIA_NamePropertyId name flags&1)
ef=ew.FindFirst(UIA.TreeScope_Subtree pc)
ret ef
