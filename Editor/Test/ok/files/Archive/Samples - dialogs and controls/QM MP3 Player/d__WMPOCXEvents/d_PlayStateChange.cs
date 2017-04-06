 /PLA_Main
function NewState ;;WMPLib.IWMPPlayer4'd

typelib WMPLib
PLA_DATA- d

 out NewState
sel NewState
	case wmppsPlaying _s="Pause"
	case else _s="Play"
_s.setwintext(id(7 d.hdlg))

 play next track if current track ended
if(NewState=wmppsMediaEnded) PostMessage d.hdlg WM_COMMAND 10 0
