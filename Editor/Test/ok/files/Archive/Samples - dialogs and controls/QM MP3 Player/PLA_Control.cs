 /PLA_Main
function action ;;action: 0 stop, 1 play/pause, 2 backward, 3 forward

typelib WMPLib
PLA_DATA- d
int i

sel action
	case 0 ;;Stop
	if(d.iPlaying<0) ret
	d.p.controls.stop
	ret
	
	case 1 ;;Play/Pause
	 out d.p.playState
	sel d.p.playState
		case [wmppsUndefined,wmppsStopped]
		i=SendMessage(d.hlv LVM_GETNEXTITEM -1 LVNI_SELECTED)
		if(i<0 and d.a.len) i=0
		case wmppsPaused d.p.controls.play; ret
		case wmppsPlaying d.p.controls.pause; ret
		case else ret
	
	case 2 ;;Back
	i=d.iPlaying-1
	if(i<0) i=d.a.len-1
	
	case 3 ;;Forward
	i=d.iPlaying+1
	if(i>=d.a.len) i=0

PLA_Control 0

LVITEM li.stateMask=LVIS_FOCUSED|LVIS_SELECTED
SendMessage d.hlv LVM_SETITEMSTATE -1 &li
if(i<0 or i>=d.a.len) ret
li.state=LVIS_FOCUSED|LVIS_SELECTED
SendMessage d.hlv LVM_SETITEMSTATE i &li
SendMessage d.hlv LVM_ENSUREVISIBLE i 0

d.iPlaying=i
d.p.URL=d.a[i].path
 d.p.controls.play
