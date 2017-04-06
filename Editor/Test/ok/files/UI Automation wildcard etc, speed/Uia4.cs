 /
function'UIA.IUIAutomationElement x y [controlType] [$name] [flags] ;;flags: 1 wildcard


UIA.CUIAutomation u._create
UIA.IUIAutomationElement ew
UIA.tagPOINT p.x=x; p.y=y
ew=u.ElementFromPoint(p); if(!ew) ret
 PN
if(controlType=0 and empty(name)) ret ew

ret _Uia(u ew controlType name flags)
