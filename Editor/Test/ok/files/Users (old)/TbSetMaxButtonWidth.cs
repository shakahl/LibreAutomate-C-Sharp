 /
function hwndTb maxButtonWidth

 Set toolbar button max width when 'equal buttons' is unchecked.
 Note that checking/unchecking 'equal buttons' removes the button width limit.

 Not needed for QM 2.2.1.4 and above.

 EXAMPLE
 TbSetMaxButtonWidth win("TOOLBAR28" "QM_toolbar") 100


int i h=id(9999 hwndTb)
TBBUTTONINFO t.cbSize=sizeof(t)
t.dwMask=TBIF_SIZE
for i 0 SendMessage(h TB_BUTTONCOUNT 0 0)
	SendMessage h TB_GETBUTTONINFO i &t
	if(t.cx<maxButtonWidth) continue
	t.cx=maxButtonWidth
	SendMessage h TB_SETBUTTONINFO i &t
