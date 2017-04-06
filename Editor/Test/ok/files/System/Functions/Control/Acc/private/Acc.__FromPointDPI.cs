function x y

 Same as FromXY, but with many objects works correctly on Win8.1+ in DPI-scaled windows.
 Still does not work correctly with many objects, eg almost all Windows controls and window parts, but works in IE, FF, Chrome, many other non-Windows objects.


#opt nowarnings 1
#if _winver>=0x601 ;;no UIA typelib on XP/Vista
if _winver>=0x603 and DpiIsWindowScaled(win(x y))
	 get IUIAutomationElement from point and convert to Acc. It fails with many objects, but works in web browsers.
	UIA.CUIAutomation u._create
	UIA.tagPOINT p.x=x; p.y=y
	UIA.IUIAutomationElement e=u.ElementFromPoint(p)
	UIA.IUIAutomationLegacyIAccessiblePattern k=e.GetCurrentPattern(UIA.UIA_LegacyIAccessiblePatternId)
	a=k.GetIAccessible; elem=0
	if(a) ret
	err+
#endif

this=acc(x y 0)
err end ERR_OBJECTGET
