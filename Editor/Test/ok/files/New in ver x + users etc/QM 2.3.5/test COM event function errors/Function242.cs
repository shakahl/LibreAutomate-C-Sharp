\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib ___MediaPlayer {22D6F304-B0F6-11D0-94AB-0080C74C7E95} 1.0

ShowDialog("" &Function242 0 0 17)
MessageLoop
 ShowDialog("" &Function242)

 BEGIN DIALOG
 0 "" 0x90CA0048 0x40180 0 0 140 28 "QM Media Player"
 99 ActiveX 0x54000000 0x4 0 0 140 28 "___MediaPlayer.MediaPlayer {22D6F312-B0F6-11D0-94AB-0080C74C7E95}"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ret
 messages
int- t_playcount t_int
sel message
	case WM_INITDIALOG
	___MediaPlayer.MediaPlayer me999._getcontrol(id(99 hDlg))
	me999.ShowPositionControls=0
	me999._setevents("me999__MediaPlayerEvents")
	
	me999.AutoStart=TRUE
	me999.FileName=_s.expandpath("$windows$\Media\chimes.wav") ;;load and play
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDCANCEL
	 close
	DT_Cancel hDlg
	PostQuitMessage 0
ret 1
