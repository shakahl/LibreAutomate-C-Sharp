int w=win("Notepad++" "Notepad++")
int c=child("" "Scintilla" w)

str textToFind="Verdana"
 ____________________________

SCI.Sci_TextToFind ttf
__ProcessMemory m
int nb=sizeof(ttf)+textToFind.len+1
lpstr p=m.Alloc(c nb)
ttf.chrg.cpMin=0; ttf.chrg.cpMax=1000000000; ttf.lpstrText=p+sizeof(ttf)
str pack.fromn(&ttf sizeof(ttf) textToFind textToFind.len)
m.Write(pack nb)
out SendMessage(c SCI.SCI_FINDTEXT 0 p)
