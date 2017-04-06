 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "503 506 508 509 505"
str e503 cb506 c508Blo c509Slo c505Add

TO_FavSel wParam cb506 "Send keys[]Press (down only)[]Release (up only)[]If key pressed[]If key not pressed[]If key toggled[]If key not toggled"
int flags; rget flags "Keys flags" "\Tools" 0 1; c505Add=flags&1

if(!ShowDialog("" &TO_Keys &controls _hwndqm)) ret

flags=val(c505Add); rset flags "Keys flags" "\Tools"
 ________________________

str s scomm
int i op=val(cb506)

e503.trim
if(flags&1 and e503.len) __QmKeysToText e503 &scomm; if(scomm=e503) scomm.all

if op<3
	if(!e503.len) e503="keys"
	
	sel op
		case 0 s="key "
		case 1 s="key+ "
		case 2 s="key- "
	s+e503
else
	e503.findreplace(" ")
	e503.findreplace("," "<")
	sel op
		case 3 s=F"ifk({e503})"
		case 4 s=F"ifk-({e503})"
		case 5 s=F"ifk({e503} 1)"
		case 6 s=F"ifk-({e503} 1)"

if(scomm.len) i=16-s.len; s.formata("%.*m;; %s" iif(i>2 i 2) ' ' scomm)

if op<3
	if(c508Blo=1) s=F"BlockInput 1[]{s}[]BlockInput 0"
	if(c509Slo=1) s=F"opt slowkeys 1; spe 100[]{s}[]opt slowkeys 0"
else
	s+"[][9]"

 sub_to.TestDialog s
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 270 177 "Keys"
 503 Edit 0x54030080 0x204 4 4 262 14 ""
 512 QM_DlgInfo 0x54000000 0x0 4 22 262 18 "Add key codes. Also can include ''text'', wait, etc. Spaces optional."
 27 Button 0x54032000 0x0 6 50 30 14 "Esc"
 9 Button 0x54032000 0x0 6 66 30 14 "Tab"
 20 Button 0x54032000 0x0 6 80 30 14 "Caps L"
 16 Button 0x54032000 0x0 6 94 30 14 "Shift"
 17 Button 0x54032000 0x0 6 108 30 14 "Ctrl"
 605 Button 0x54032000 0x0 42 50 48 14 "F1 - F12"
 601 Button 0x54032000 0x0 42 66 48 14 "0 - 9"
 602 Button 0x54032000 0x0 42 80 48 14 "A - Z"
 603 Button 0x54032000 0x0 42 94 48 14 "/\`-=][;',."
 18 Button 0x54032000 0x0 36 108 30 14 "Alt"
 32 Button 0x54032000 0x0 66 108 30 14 "Space"
 8 Button 0x54032000 0x0 96 66 30 14 "Back"
 13 Button 0x54032000 0x0 96 80 30 14 "Enter"
 93 Button 0x54032000 0x0 96 94 30 14 "App"
 91 Button 0x54032000 0x0 96 108 30 14 "Win"
 44 Button 0x54032000 0x0 132 50 26 14 "Pr Sc"
 45 Button 0x54032000 0x0 132 66 26 14 "Ins"
 46 Button 0x54032000 0x0 132 80 26 14 "Del"
 37 Button 0x54032000 0x0 132 108 26 14 "Left"
 145 Button 0x54032000 0x0 158 50 26 14 "Scr L"
 36 Button 0x54032000 0x0 158 66 26 14 "Home"
 35 Button 0x54032000 0x0 158 80 26 14 "End"
 38 Button 0x54032000 0x0 158 94 26 14 "Up"
 40 Button 0x54032000 0x0 158 108 26 14 "Down"
 19 Button 0x54032000 0x0 184 50 26 14 "Pause"
 33 Button 0x54032000 0x0 184 66 26 14 "Pg Up"
 34 Button 0x54032000 0x0 184 80 26 14 "Pg Dn"
 39 Button 0x54032000 0x0 184 108 26 14 "Right"
 144 Button 0x54032000 0x0 216 50 30 14 "Num L"
 604 Button 0x54032000 0x0 216 66 48 14 "Numpad"
 606 Button 0x54032000 0x0 216 94 48 14 "Text"
 607 Button 0x54032000 0x0 216 108 48 14 "(...)"
 510 Static 0x54000000 0x0 6 134 26 10 "Action"
 506 ComboBox 0x54230243 0x4 36 132 90 213 ""
 508 Button 0x54012003 0x4 130 132 68 13 "Block user input" "Adds:[]BlockInput 1"
 509 Button 0x54012003 0x4 200 132 50 13 "Slow" "Adds:[]opt slowkeys 1"
 513 QM_DlgInfo 0x54000000 0x20000 130 134 136 16 ""
 505 Button 0x54012003 0x0 200 158 66 13 "Add comments" "Add comments with key names, like[]key CT ;;Ctrl+Tab"
 1 Button 0x54030001 0x4 6 158 48 14 "OK"
 2 Button 0x54010000 0x4 58 158 50 14 "Cancel"
 511 Button 0x54032000 0x4 112 158 18 14 "?"
 507 Button 0x54032000 0x0 134 158 36 14 "Info..."
 3 Static 0x54000010 0x20000 0 152 286 1 ""
 504 Button 0x54020007 0x0 2 42 266 84 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 messages
if(sub_to.ToolDlgCommon(&hDlg "506[]$qm$\keyboard.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	TO_ButtonsAddArrow hDlg "601-607"
	goto g11
	case WM_COMMAND goto messages2
ret
 messages2
int k he=id(503 hDlg)
if(wParam&0xffff0000=0) SetWinStyle lParam BS_DEFPUSHBUTTON 2
sel wParam
	case 511 QmHelp "IDP_KEY[]*[]*[]IDP_IFK[]*[]*[]*" TO_Selected(hDlg 506)
	case 507
	sel ShowMenu("1 QM key codes[]2 Virtual-key codes[]3 Common keyboard shortcuts (QM Help)[]4 Common keyboard shortcuts (Google)" hDlg)
		case 1 QmHelp "IDP_KEYCODES"
		case 2 QmHelp "IDP_VIRTUALKEYS"
		case 3 QmHelp "IDP_KEYSHORTCUTS"
		case 4 run "http://www.google.com/search?q=Windows%20keyboard%20shortcuts"
	
	case CBN_SELENDOK<<16|506
	 g11
	i=TO_Selected(hDlg 506)
	TO_Show hDlg "508 509 -513" i<=2
	sel(i) case [3,4] s="Can be 1 or 2 keys."; case [5,6] s="Caps Lock, Num Lock or Scroll Lock."
	s.setwintext(id(513 hDlg))
	
	case EN_CHANGE<<16|503
	s.getwintext(lParam)
	__QmKeysToText s &_s
	_s.setwintext(id(512 hDlg))
	
	case 601 k=ShowMenu("0[]1[]2[]3[]4[]5[]6[]7[]8[]9" hDlg 0 2); if(k) k+'0'-1; goto gKeys
	case 602 k=ShowMenu("A[]B[]C[]D[]E[]F[]G[]H[]I[]J[]K[]L[]M[]N[]O[]P[]Q[]R[]S[]T[]U[]V[]W[]X[]Y[]Z" hDlg 0 2); if(k) k+'A'-1; goto gKeys
	case 603 k=ShowMenu("`~[] -_[]=+[]]}[][{[];:[][39]''[],<[].>[]/?[]\|" hDlg 0 2); if(k and QmKeyCodeToVK(" `-=][;',./\"+k &k)) goto gKeys
	case 604 k=ShowMenu("0[]1[]2[]3[]4[]5[]6[]7[]8[]9[]*[]+[] -[].[]/" hDlg 0 2); if(k) k+VK_NUMPAD0-(k<13); goto gKeys
	case 605 k=ShowMenu("F1[]F2[]F3[]F4[]F5[]F6[]F7[]F8[]F9[]F10[]F11[]F12" hDlg 0 2); if(k) k+VK_F1-1; goto gKeys
	
	case 606 ;;Text
	i=ShowMenu("1 Text begin/end[9]''[]2 New line (Enter)[9][91]][]3 Double quote ('')[9][39]'[]4 Tab[9][91]9][]-[]5 Begin text with variables[9]F''[]6 Variable[9]{var}" hDlg)
	sel i
		case 0
		case 6 
		if inp(s "Variable" "" "" 0 "" 0 hDlg) and s.len
			s-"{"; s+"}"; goto gText
		case else
		s.getl("''[][91]][][39]'[][91]9][]F''" i-1); goto gText
	
	case 607 ;;(...)
	i=ShowMenu("7 String variable[9](var)[]1 Wait[9](0.1)[]2 Repeat[9](#n)[]3 VK constant (part 1)[9](VK_X)[]4 VK constant (part 2)[9](VK_X)[]5 Virtual-key code[9](vk)[]6 Scan code[9](0x10000|sc)" hDlg)
	sel i
		case 1
		if inp(s "Number of seconds to wait. Can be a number or a variable of type double." "" "0.1" 0 "" 0 hDlg) and s.len
			if(isdigit(s[0]) and findc(s '.')<0) s+".0"
			s-"("; s+")"
		case 2
		if inp(s "Number of times to repeat the key" "" "1" 0 "" 0 hDlg) and s.len
			s-"(#"; s+")"
		case [3,4]
			sub_to.VirtualKeysMenu he i=4 0 s
		case 5
		if inp(s "Virtual-key code, 8 to 255 (0xFF).[]Can be an integer number or variable." "" "" 0 "" 0 hDlg) and s.len
			s-"("; s+")"
		case 6
		if inp(s "Physical key scan code, 1 to 127 (0x7F).[]Can be an integer number or variable." "" "" _i "Extended key" 0 hDlg) and s.len
			if(_i) _i=0x20000
			s-F"(0x{0x10000|_i}|"; s+")"
		case 7
		if inp(s "Str or lpstr variable. Will be typed as text." "" "" 0 "" 0 hDlg) and s.len
			s-"("; s+")"
	if(s.len) goto gText
	
	case else
	if(wParam>=8 and wParam<=255) k=wParam; goto gKeys
ret 1

 gKeys
QmKeyCodeFromVK k &s

 gText
EditReplaceSel he 0 s 2
