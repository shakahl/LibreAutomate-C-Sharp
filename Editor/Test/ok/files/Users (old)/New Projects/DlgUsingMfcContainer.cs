\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib MediaPlayer {22D6F304-B0F6-11D0-94AB-0080C74C7E95} 1.0

dll- qmcc
	#CcCreate hParent GUID*clsid $licKey
	CcDestroy c
	CcGet c GUID*iid !**ctrl

int- c

out
ShowDialog("DlgUsingMfcContainer" &DlgUsingMfcContainer 0)

 BEGIN DIALOG
 0 "" 0x10CA0044 0x100 0 0 205 104 "QM Media Player"
 99 Static 0x50000000 0x4 0 0 208 86 ""
 3 Button 0x54032000 0x0 0 90 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	goto play
	
	case WM_DESTROY
	CcDestroy c
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	goto play
ret 1

 play
MediaPlayer.MediaPlayer me99
c=CcCreate(id(99 hDlg) uuidof(MediaPlayer.MediaPlayer) 0)
 out c
CcGet(c uuidof(MediaPlayer.IMediaPlayer2) &me99)

_s.expandpath("Q:\MP3\Happy Rhodes - Case Of Glass.mp3")
me99.FileName=_s
