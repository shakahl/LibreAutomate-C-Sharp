 Main function of MIDI triggers.

if(getopt(nthreads)>1) ret

str+ _mt_menu
rget _mt_menu "MIDI triggers menu"
if(!_mt_menu.len) _mt_menu="MT_Menu"

str controls = "6"
str Edit6(_mt_menu)
int+ _hwndmt=ShowDialog("MT_Dialog" &MT_Dialog &controls 0 1 0 WS_VISIBLE|DS_SETFOREGROUND)
err ret
MessageLoop
