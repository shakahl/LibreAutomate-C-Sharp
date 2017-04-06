out
PF

 int w2=win("QM TOOLBAR" "QM_toolbar")
 IUIAutomationElement e=Uia(w2 "Mouse")

 IUIAutomationElement e=Uia(win("Internet Explorer") "Spausdinti")

int w1=child("" "Internet Explorer_Server" win("Internet Explorer" "IEFrame"))
IUIAutomationElement e=Uia(w1 "Veikla")

 IUIAutomationElement e=Uia(win("- Mozilla Firefox" "MozillaUIWindowClass") "Nefiltruota")

 int w1=child("" "MozillaWindowClass" win("- Mozilla Firefox" "MozillaUIWindowClass") 0 0 0 5)
 IUIAutomationElement e=Uia(w1 "Highlight find results")

PN
if e
	 e=e.GetCachedParent ;;0
	UIA.IUIAutomationLegacyIAccessiblePattern p=e.GetCurrentPattern(UIA.UIA_LegacyIAccessiblePatternId)
	Acc a.a=p.GetIAccessible
	out a.a ;;0 for some, eg toolbar buttons (maybe for those with elem). OK in Firefox an IE. Speed 2 ms.
	out p.CurrentName
	 out a.Name
PO

if(e) out e.CurrentName
else out "---- not found -----"



e.GetCurrentPropertyValue
e.CurrentItemStatus
