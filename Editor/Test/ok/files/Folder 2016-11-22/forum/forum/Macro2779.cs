int w=win("New Tab - Google Chrome" "Chrome_WidgetWin_1")
act w
Acc a.Find(w "TEXT" "Address and search bar" "class=Chrome_WidgetWin_1[]state=0x100000 0x20000040" 0x1005)
a.SetValue("Some value") ;;works but does not start searching; then I tried DoDefaultAction and Enter, and it worked; also tried Select, but Chrome says 'not implemented'
a.DoDefaultAction
key Y
