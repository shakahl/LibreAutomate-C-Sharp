/exe

int w=win("Notepad")
if(w) act w; else run "notepad.exe"; 1; w=win("Notepad")
 w=id(15 w) ;;can attach to controls too

mac "dlg_attached" w

#if EXE
mes "Exit exe."
#endif
