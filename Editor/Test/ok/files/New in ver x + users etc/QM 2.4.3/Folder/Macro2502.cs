 IAppVisibility

 outw win(mouse)

 int w=_hwndqm
 int w=win("Notepad" "Notepad")
  out IsWindowVisible(w)
 
 def DWMWA_CLOAKED 14
 int i
 if(DwmGetWindowAttribute(w DWMWA_CLOAKED &i 4)) end "error"
 out i

int w=GetForegroundWindow; outw w
outw RealGetNextWindow(w 1)
