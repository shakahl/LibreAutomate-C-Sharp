 /
function'UIA.IUIAutomationElement hwnd [controlType] [$name] [flags] ;;flags: 1 wildcard

UIA.CUIAutomation u._create
UIA.IUIAutomationElement ew
ew=u.ElementFromHandle(+hwnd); if(!ew) ret
if(controlType=0 and empty(name)) ret ew

ret _Uia(u ew controlType name flags)
