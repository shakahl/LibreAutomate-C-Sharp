 /
function'UIA.IUIAutomationElement Acc&parent [controlType] [$name] [flags] ;;flags: 1 wildcard

 UIA bug: when we get parent from IAccessible (not from HWND), then FindFirst etc is about 60 times slower.


UIA.CUIAutomation u._create
UIA.IUIAutomationElement ew
ew=u.ElementFromIAccessible(parent.a parent.elem); if(!ew) ret
 ew=u.ElementFromIAccessibleBuildCache(parent.a parent.elem); if(!ew) ret ;;not tested, need IUIAutomationCacheRequest
 PN
if(controlType=0 and empty(name)) ret ew

ret _Uia(u ew controlType name flags)
