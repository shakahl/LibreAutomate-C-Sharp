 /
function'UIA.IUIAutomationElement Acc&a

 Note: fails with most objects. Eg with Windows controls and their children. It is documented. Works with Firefox.

UIA.CUIAutomation u._create
UIA.IUIAutomationElement e=u.ElementFromIAccessible(a.a a.elem) ;;very slow
ret e
