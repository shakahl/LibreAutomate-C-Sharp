 open www.download.com and wait until fully loads
web "http://www.download.com/" 0x1
 set search text and click Go button
Htm el=htm("INPUT" "qt" "" "Download.com" 0 5 0x221)
el.SetText("Quick Macros")
el=htm("INPUT" "searchGo" "" "Download.com" 0 6 0x121)
el.Click
 wait for element "Results" max 60 seconds
el=htm("H1" "Results" "" "Download.com" 0 0 0x21 60)
 wait until fully loads
wait 0 I
 done
mes "Done"

 http://www.quickmacros.com/forum/viewtopic.php?t=796
