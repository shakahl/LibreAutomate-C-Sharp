 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "5 1204 1201 1206 1304 1305 1301 1309 1102 1104 1001 1402 1404 1406"
__strt lb5 c1204Wai e1201Fil c1206No cb1304Pla cb1305Sho e1301Fil c1309No e1102Fre e1104Dur cb1001Sou e1402Tex c1404Que c1406Syn

TO_FavSel wParam lb5 "Default sound[]PC speaker[]Short wave file[]Play audio[]Speak text"
cb1001Sou="&Default[]Error[][]Warning[]Information"
e1102Fre=1000; e1104Dur=1000
cb1304Pla="Only load file[]&Play[]Stop[]Pause[]Loop[]Play while macro runs"
cb1305Sho="&[]Show[]Hide"

if(!ShowDialog("" &TO_Sound &controls _hwndqm)) ret

int i
str s
sel val(lb5)
	case 0 s=F"bee {val(cb1001Sou)}"; sub_to.Trim s " 0"
	case 1 s=F"bee {e1102Fre.N(`1000`)} {e1104Dur.N(`1000`)}"
	case 2 s=F"bee{c1204Wai.SelC(` +`)} {e1201Fil.S}"
	case 3 s=F"Play {val(cb1304Pla)} {e1301Fil.S} {val(cb1305Sho)}"; sub_to.Trim s " '''' 0"
	case 4
		TO_FlagsFromCheckboxes _s c1406Syn 1 c1404Que 2
		s=F"Speak {e1402Tex.S} {_s}"; sub_to.Trim s " 0"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 309 130 "Sound"
 5 ListBox 0x54230101 0x204 4 4 84 90 ""
 1205 Static 0x44020000 0x4 100 4 24 10 "File"
 1201 Edit 0x44010080 0x204 100 16 206 14 "Fil"
 1202 Button 0x44012000 0x4 100 32 50 14 "Browse..."
 1203 Button 0x44032000 0x4 150 32 18 14 "SF" "Special folders"
 1206 Button 0x44012003 0x0 182 34 48 12 "No SF" "Let the button give me normal path, not special folder name"
 1204 Button 0x44012003 0x4 100 56 100 12 "Wait until the sound ends"
 1306 Static 0x44020000 0x4 100 6 28 12 "Action"
 1304 ComboBox 0x44230243 0x4 130 4 96 213 "Pla"
 1307 Static 0x44020000 0x4 100 22 28 12 "Player"
 1305 ComboBox 0x44230243 0x4 130 20 96 213 "Sho"
 1308 Static 0x44020000 0x4 100 40 40 10 "File"
 1301 Edit 0x44010080 0x204 100 50 204 14 "Fil"
 1302 Button 0x44012000 0x4 100 66 50 14 "Browse..."
 1303 Button 0x44032000 0x4 152 66 18 14 "SF" "Special folders"
 1309 Button 0x54012003 0x0 178 66 48 12 "No SF" "Let the button give me normal path, not special folder name"
 1101 Static 0x44020000 0x4 100 6 96 12 "Frequency, 37...32767 Hz"
 1102 Edit 0x44030080 0x204 198 4 50 14 "Fre"
 1103 Static 0x44020000 0x4 100 24 96 12 "Duration, ms"
 1104 Edit 0x44030080 0x204 198 22 50 14 "Dur"
 1105 QM_DlgInfo 0x54000000 0x20000 100 68 206 26 "Note: On some computers may use soundcard instead. On some computers may not work. Read more in Help."
 1001 ComboBox 0x44230243 0x4 100 4 96 213 "Sou"
 1 Button 0x54030001 0x4 4 110 50 14 "OK"
 2 Button 0x54010000 0x4 56 110 50 14 "Cancel"
 6 Button 0x54032000 0x4 108 110 16 14 "?"
 12 Button 0x54032000 0x4 204 110 50 14 "Play"
 14 Button 0x54032000 0x4 256 110 50 14 "Stop"
 1401 Static 0x44000000 0x4 100 6 26 10 "Text"
 1402 Edit 0x44231044 0x204 128 4 176 30 "Tex"
 1404 Button 0x44012003 0x4 186 38 118 12 "Queue (wait if already speaking)"
 1406 Button 0x44012003 0x4 100 38 80 12 "Synchronous (wait)"
 1405 QM_DlgInfo 0x54000000 0x20000 100 58 206 36 "You can also set voice, speed and volume. Click ? to read more."
 15 Static 0x54000010 0x20004 0 102 316 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "5[]$qm$\sound.ico" "1201 1301" 1)) ret wParam
sel message
	case WM_INITDIALOG goto g1
	case WM_DESTROY bee ""; Play 2
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 1202 bee ""; Play 2; sub_to.FileDialog hDlg 1201 "sounddir" "$Windows$\Media" "wav[]*.wav[]" "wav"
	case 1203 sub_to.File_SF hDlg 1201 "\"
	case 1302 bee ""; Play 2; sub_to.FileDialog hDlg 1301 "sounddir2" "$Windows$\Media" "All files[]*.*[]"
	case 1303 sub_to.File_SF hDlg 1301 "\"
	case 12 ;; Play
	Play 2
	sel TO_Selected(hDlg 5)
		case 0 bee TO_Selected(hDlg 1001)
		case 1 bee GetDlgItemInt(hDlg 1102 0 0) 1000
		case 2 bee s.getwintext(id(1201 hDlg))
		case 3 Play 1 s.getwintext(id(1301 hDlg))
		case 4
		s.getwintext(id(1402 hDlg))
		if(s.len>60) s.fix(50); s+", and so on."
		mac "Speak" "" s
	case 14 bee ""; Play 2 ;; Stop
	case 6 QmHelp "IDP_BEE[]*[]*[]Play[]Speak" TO_Selected(hDlg 5)
	
	case LBN_SELCHANGE<<16|5
	 g1
	i=TO_Selected(hDlg 5)
	DT_Page hDlg i
	
	case CBN_SELENDOK<<16|1304
	i=TO_Selected(hDlg 1304)
	TO_Enable hDlg "1301-1303" (i!1 and i!2)
ret 1

#opt nowarnings 1
