 int w=win("Options" "bosa_sdm_Microsoft Office Word 11.0")
 Acc a.Find(w "CHECKBUTTON" "Highlight" "class=bosa_sdm_Microsoft Office Word 11.0" 0x1005)
 ----
int w=wait(2 WV win("Google Išplėstinė paieška - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a.FindFF(w "SELECT" "" "name=as_occt" 0x1004 2)

typelib UIA {944DE083-8FB8-45CF-BCB7-C477ACB2F897} 1.0 0 2

Q &q
UIA.CUIAutomation u._create
Q &qq
rep 10
	UIA.IUIAutomationElement e=u.ElementFromIAccessible(a.a a.elem) ;;very slow
Q &qqq
ARRAY(int) z=e.GetRuntimeId
Q &qqqq
outq
