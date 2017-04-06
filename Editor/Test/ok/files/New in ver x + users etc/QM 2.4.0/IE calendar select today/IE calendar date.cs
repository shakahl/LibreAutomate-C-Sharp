int w=wait(3 WV win("Handover - Windows Internet Explorer" "IEFrame"))
act w

 first calendar icon
Htm e=htm("IMG" "*HANDOVER_CAL_TYPE*" "" w "0" 21 0x821 3)
e.ClickAsync ;;don't use Click, it waits until the date dialog closed
int w2=wait(15 win(" -- Webpage Dialog" "Internet Explorer_TridentDlgFrame"))

 click 'today' link
e=htm("A" "" "<a class=''day* today*" w2 "0" 15 0x24 10) ;;note: added 10 s waiting
e.Click

0.5 ;;avoid 'popup is blocked' message when clicked the second icon too fast

 other calendar icon
e=htm("IMG" "*REGISTRATION_CAL_TYPE*" "" w "0" 22 0x821 3)
e.ClickAsync
w2=wait(15 win(" -- Webpage Dialog" "Internet Explorer_TridentDlgFrame"))

 click 'today' link
e=htm("A" "" "<a class=''day* today*" w2 "0" 15 0x24 10) ;;note: added 10 s waiting
e.Click
