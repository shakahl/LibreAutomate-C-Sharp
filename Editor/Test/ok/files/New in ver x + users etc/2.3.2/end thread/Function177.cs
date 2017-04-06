 /Macro1403
int h=win("Quick")
 dis "Dialog58"
 act h
 clo h
 PostMessage h WM_CLOSE 0 0; 0.5
 SendMessage h WM_CLOSE 0 0
 RECT r; GetWindowRect h &r; InflateRect &r -1 0; siz r.right-r.left r.bottom-r.top h
 RECT r; GetWindowRect h &r; InflateRect &r -10 0; SetWindowPos h 0 0 0 r.right-r.left r.bottom-r.top SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE
 PostMessage h WM_USER 0 0; 0.5
 Sleep 2000
Sleep 1500
