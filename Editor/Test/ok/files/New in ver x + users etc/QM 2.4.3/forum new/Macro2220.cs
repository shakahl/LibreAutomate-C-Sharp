 int w=win(mouse)
 outw w
 out IsShellWindow(w)

 int w=win(mouse)
 if(IsShellWindow(w)<1) clo w; err
