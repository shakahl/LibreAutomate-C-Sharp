int t1=perf
int hwnd=child("" "MozillaWindowClass" win(" - Mozilla Firefox" "MozillaUIWindowClass") 0x1 0 0 2)
Acc a=acc("Submit" "PUSHBUTTON" hwnd "" "" 0x1001)
int t2=perf
out t2-t1/1000
out a.Name
