 /Macro482
function action [$_file] [show] ;;action: 0 none, 1 play, 2 stop, 3 pause, 4 loop, 5 play while macro runs;  show: 0 none, 1 show, 2 hide, -1 close player.

 Plays audio file.

 Deb
int hf h=win("QM Media Player" "QM_Media_Player")
if(!h)
	if(show=-1) ret
	sel(action) case [2,3] if(show!=1) ret
	hf=win
	run "qmplayer.exe"
	h=wait(30 WC win("QM Media Player" "QM_Media_Player"))
	
sel show
	case 1 if(hid(h)) Zorder h win; hid- h; show=-1
	case 2 hid h
	case -1 clo h; ret

sel action
	case [0,1,4,5]
	sel action
		case 0 if(!len(_file)) ret
		case 5 action=4; atend PlayStop show<0
	_s.expandpath(_file)

COPYDATASTRUCT cd.dwData=action; cd.cbData=_s.len+1; cd.lpData=_s
SendMessage h WM_COPYDATA 0 &cd
 if(hf and win=h) act hf

err+;; out _error.description
