 ----
 int w=wait(10 WV win("Quick Macros Forum • Post a new topic - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 Acc a.FindFF(w "A" "Administration Control Panel" "" 0x1001 10)
 ----
 int w=wait(10 WV win("comparison - Definition from Longman English Dictionary Online - Windows Internet Explorer" "IEFrame"))
 Acc a.Find(w "TEXT" "FOOD" "state=0x40 0x20000040" 0x3085|16)
 ----
 int w=wait(10 WV win("Inbox - Local Folders - Mozilla Thunderbird" "Mozilla*WindowClass" "" 0x804))
 Acc a.FindFF(w "A" "http://core.surpasshosting.com" "" 0x1001 10)
 ----
 int w=win("Quick Macros" "QM_Editor")
 Acc a.Find(w "OUTLINEITEM" "FFNode.ScrollTo" "class=SysTreeView32[]id=2202" 0x1015)
 ----
 int w=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Windows Internet Explorer" "IEFrame"))
 Acc a.Find(w "LINK" "čia" "" 0x3011 10)
 ----
 int w=wait(10 WV win("LHMT _ Skaitmeninė orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
 Acc a.FindFF(w "A" "čia" "" 0x1001 10)


a.WebScrollTo
