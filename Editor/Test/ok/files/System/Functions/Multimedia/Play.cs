 /
function action [$_file] [show] ;;action: 0 none, 1 play, 2 stop, 3 pause, 4 loop, 5 play while macro runs;  show: 0 none, 1 show, 2 hide, -1 close player.

 Plays audio file (music).

 _file - file. Can be mp3, wav or other format supported by the Windows Media Player.

 REMARKS
 Note: Does not wait until finished playing. In exe the sound stops as soon as exe process ends.


int+ ___hwndmp
if(!IsWindow(___hwndmp))
	___hwndmp=0
	if(show=-1) ret
	sel(action) case [2,3] if(show!=1) ret
	int hf=win
	mac "sub.Thread"
	opt waitmsg -1; wait 10 V ___hwndmp; err end ERR_FAILED

sel show
	case 1 if(hid(___hwndmp)) Zorder ___hwndmp win; hid- ___hwndmp; show=-1
	case 2 hid ___hwndmp
	case -1 clo ___hwndmp; ret

sel action
	case [0,1,4,5]
	sel action
		case 0 if(empty(_file)) ret
		case 5 action=4; atend PlayStop show<0
	_s.expandpath(_file)

SendMessage ___hwndmp WM_SETTEXT action _s
if(hf and win=___hwndmp) act hf

err+


#sub Thread
typelib ___MediaPlayer {22D6F304-B0F6-11D0-94AB-0080C74C7E95} 1.0

str dd=
 BEGIN DIALOG
 0 "" 0x90CA0048 0x40180 0 0 140 28 "QM Media Player"
 99 ActiveX 0x54000000 0x4 0 0 140 28 "___MediaPlayer.MediaPlayer {22D6F312-B0F6-11D0-94AB-0080C74C7E95}"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ShowDialog(dd &sub.DlgProc 0 0 17 WS_VISIBLE)
MessageLoop


#sub DlgProc
function# hDlg message wParam lParam
int- t_playcount t_int
sel message
	case WM_INITDIALOG
	___MediaPlayer.MediaPlayer me99._getcontrol(id(99 hDlg))
	me99.ShowPositionControls=0
	me99._setevents("sub.")
	int+ ___hwndmp=hDlg
	
	case WM_USER+10 if(t_int) goto g1
	case WM_SETTEXT
	t_int=0
	 g1
	err-
	lpstr _sfile=+lParam; str sfile=_sfile
	me99._getcontrol(id(99 hDlg))
	sel wParam
		case 0 ;;only load file
		me99.AutoStart=0; me99.FileName=sfile
		
		case [1,4]
		if(wParam&1) t_playcount=1; else t_playcount=0; t_int=1 ;;me99.PlayCount fails on XP
		me99.AutoStart=TRUE
		if(sfile.len) me99.FileName=sfile ;;load and play
		else me99.Play ;;already loaded
		
		case 2 me99.Stop
		case 3 me99.Pause
	err+ out _error.description
	ret 1
	
	case WM_TIMER
	if(wParam=1)
		if(hid(hDlg)) goto close ;;if hidden, close after 10 minutes of silence
		else KillTimer hDlg wParam
	
	case WM_DESTROY
	___hwndmp=0
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDCANCEL
	 close
	DT_Cancel hDlg
	PostQuitMessage 0
ret 1


#sub _PlayStateChange
function OldState NewState ___MediaPlayer.IMediaPlayer2'me99

if(NewState=___MediaPlayer.mpStopped)
	me99.CurrentPosition=0; err
	SetTimer ___hwndmp 1 600000 0


#sub _EndOfStream
function Result ___MediaPlayer.IMediaPlayer2'me99

int- t_playcount
if(!t_playcount)
	me99.CurrentPosition=0; err
	PostMessage ___hwndmp WM_USER+10 4 0 ;;me99.Play fails on Vista+
