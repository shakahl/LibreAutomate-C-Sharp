 Useless, because:
 Unavailable in web pages.
 Unavailable on Win2000.

 EXAMPLES

#compile "____AccIdentity"
__AccIdentity x

 ----
 int w=win("" "Shell_TrayWnd")
 Acc a.Find(w "PUSHBUTTON" "qgindi is logged in" "class=ToolbarWindow32[]id=1504" 0x1005)
 x.Init(a)
 
 Acc a1.Find(w "PUSHBUTTON" "qgindi is logged in" "class=ToolbarWindow32[]id=1504" 0x1005)

 ----
 int w=win("Calculator" "CalcFrame")
 Acc a.Find(w "PUSHBUTTON" "6" "class=Button" 0x1005)
 ----
int w=win("Options" "bosa_sdm_Microsoft Office Word 11.0")
Acc a.Find(w "CHECKBUTTON" "Highlight" "class=bosa_sdm_Microsoft Office Word 11.0" 0x1005)

x.Init(a)




 out x.IsEqualAcc(a1)
