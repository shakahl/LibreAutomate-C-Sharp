 /
function $text [color]

 Sets text in 'TB StatusBar'.
 Text color can be specified.

 EXAMPLE
 SetTbStatusBarText "Status: Updating ..." 0x0000ff
 2
 SetTbStatusBarText "Status: Finished"


int h=id(1 win("TB STATUSBAR" "QM_Toolbar")); err ret

SetWindowTextW h @text

CHARFORMAT2 cf.cbSize=sizeof(CHARFORMAT2)
cf.dwMask=CFM_COLOR
cf.crTextColor=color
SendMessage(h EM_SETCHARFORMAT SCF_ALL &cf)
