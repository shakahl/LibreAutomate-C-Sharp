int w=wait(3 WV win(" - YouTube - Google Chrome" "Chrome_WidgetWin_1"))
Acc a.FindFF(w "span" "" "class=yt-subscription-button*" 0x1004 3|4)
str s=a.Description
s.findreplace("."); s.findreplace(",") ;;remove thousand separators
int n=val(s)
out n
