out
 run "firefox" "http://www.download.com/"
 run "$pf$\Opera\opera.exe" "http://www.download.com/"
 run "chrome" "http://www.download.com/"
 1
 Acc a=acc("Read more in The Daily Download" "TEXT" win(" - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804) "" "" 0x3811 0x40 0x20000040 "" 15)
 Acc a=acc("Read more in The Daily Download" "LINK" win(" - Opera" "OperaWindowClass") "" "" 0x3001 0 0 "" 15)
Acc a=acc("" "TEXT" win(" - Google Chrome" "Chrome_WidgetWin_0") "" "Read more in The Daily Download" 0x3804 0x40 0x20000040 "" 30)
out a.Name
