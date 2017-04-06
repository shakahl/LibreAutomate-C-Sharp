int w=wait(3 WV win("Internet Explorer" "IEFrame"))
act w
Htm e=htm("B" "What can be automated?" "" w 0 2 0x21 3)
 Htm e=htm("A" "Resources" "" w 0 11 0x21 3)
 Htm e=htm("BODY" "" "" w 0 0 0x20)
 Htm e=htm("INPUT" "q" "" w 0 2 0x221 3)
 Htm e=htm("INPUT" "Search" "" w 0 3 0x421 3)
 Htm e=htm("TEXTAREA" "message" "" w 0 0 0x121 3)
 Htm e=htm("INPUT" "icon" "" w 0 2 0x121 3)
 Htm e=htm("BUTTON" "Click Me!" "" w "2" 0 0x21 3)

e.SelectText
 e.SelectText(5)
 e.SelectText(0 3)
 e.SelectText(3 2)
 e.SelectText(5 -1)

 e.SelectText(300 200)
 e.SelectText(300)
 e.SelectText(0 300)
