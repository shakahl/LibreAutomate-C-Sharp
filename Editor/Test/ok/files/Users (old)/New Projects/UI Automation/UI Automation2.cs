UIA.CUIAutomation u._create
UIA.IUIAutomationElement er ef
UIA.IUIAutomationCondition pc=u.CreatePropertyCondition(UIA.UIA_NamePropertyId "Untitled - Notepad")
er=u.GetRootElement
ef=er.FindFirst(UIA.TreeScope_Children pc)
out ef
if(ef) out ef.CurrentName
