int w1=win("Firefox" "MozillaUIWindowClass")
 int w1=win("Chrome" "")
 int w1=win(" - Opera" "OperaWindowClass")
 act w1
 0.5
Q &q
Acc a=acc("" "DOCUMENT" w1 "" "" 0x3000)
Q &qq; outq
