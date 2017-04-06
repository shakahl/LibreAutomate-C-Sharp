int w=win("Dialog" "#32770")
 PostMessage w WM_COMMAND 4 0
 5
PF
 out SendMessageTimeout(w WM_APP 0 0 SMTO_ABORTIFHUNG 10000 &_i)
out SendMessageTimeout(w WM_APP+1 0 0 SMTO_ABORTIFHUNG 10000 &_i)

 int c=id(3 w)
 PostMessage(w WM_APP 0 0); 0.1
 _s.getwintext(c)
PN;PO
 out _s
