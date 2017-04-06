UIA.CUIAutomation u._create

UIA.tagPOINT p; GetPhysicalCursorPos +&p
UIA.IUIAutomationElement e=u.ElementFromPoint(p)
 out e
 out e.CurrentClassName
 out e.CurrentItemStatus
 out e.CurrentItemType
 out e.CurrentName
 out e.CurrentNativeWindowHandle
 outw e.CachedNativeWindowHandle ;;error, parameter incorrect
 outw sub.GetHandle(e)
 ret

rep
	1
	PF
	GetPhysicalCursorPos +&p
	PN
	e=u.ElementFromPoint(p)
	 e=u.ElementFromPointBuildCache(p)
	PN
	int h=e.CurrentNativeWindowHandle
	PN
	PO
	out e.CurrentName
	outw h


#sub GetHandle
function# UIA.IUIAutomationElement'e
rep
	int h=e.CurrentNativeWindowHandle
	if(h!0) ret h
	 e=e.GetCachedParent ;;gets 0
	e=e.FindFirst(UIA.TreeScope_Parent 0) ;;cannot be 0
	out e
	if(e=0) ret
