 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!IsUserAdmin) mes "QM must run as administrator." "" "x"; ret
sub_sys.PortableWarning

str controls = "5"
str qmdi5
qmdi5=
 <>Click an empty cell and press the key or select from the list.
 Remapping will be applied after restarting Windows, or log off/on.
 It is a Windows feature and does not depend on whether QM is running. Applies to all user accounts and keyboards.
;
 To disable a key, set only From.
 To remove a remapping, right click and click Delete.
;
 To enter keys that are already remapped, use the list, not the keyboard. If you press a remapped key, it will be recorded as the new key. Or at first remove the remapping and restart Windows.
;
 <link "http://www.quickmacros.com/forum/viewtopic.php?f=1&t=5152">Other remapping methods</link>
;
if(!ShowDialog("" &TO_RemapKeyboardKeys &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 244 210 "Remap Keys"
 3 QM_Grid 0x54210001 0x200 0 0 244 130 "0,1,0,2[]From,49%,1[]To,49%,1"
 5 QM_DlgInfo 0x54000000 0x20000 0 136 244 50 ""
 1 Button 0x54030001 0x4 4 192 48 14 "OK"
 2 Button 0x54030000 0x4 54 192 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" "5"

ret
 messages
int- t_hook t_edit
int i k k1 k2 n
str s s1 s2
int* ai
DlgGrid g.Init(hDlg 3)

if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\keyboard.ico")) ret wParam
sel message
	case WM_INITDIALOG
	if(rget(s "Scancode Map" "SYSTEM\CurrentControlSet\Control\Keyboard Layout" HKEY_LOCAL_MACHINE "" REG_BINARY) and s.len>=20)
		ai=s; n=ai[2]
		if !ai[0] and n<=s.len/4-3 and n>=2
			n-1; ai+12
			for i 0 n
				k=ai[i]; k1=k>>16; k2=k&0xffff
				sub.Format s1 k1 k1&0xE000=0xE000
				sub.Format s2 k2 k2&0xE000=0xE000
				g.RowAddSetSA(i &s1 2)
	
	t_hook=SetWindowsHookEx(WH_KEYBOARD_LL &sub.Hook _hinst 0)
	
	case WM_DESTROY
	UnhookWindowsHookEx t_hook
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	ICsv c=CreateCsv
	c.FromQmGrid(id(3 hDlg))
	if(!sub_sys.RKK_Remap(c)) mes "Failed" "" "x"
ret 1
 messages3
NMHDR* nh=+lParam
GRID.QM_NMLVDATA* cd=+nh
sel nh.idFrom
	case 3
	NMLVDISPINFO* di=+nh
	sel nh.code
		case LVN_BEGINLABELEDIT
		t_edit=g.Send(LVM_GETEDITCONTROL)
		
		case LVN_ENDLABELEDIT
		t_edit=0
		
		case GRID.LVN_QG_COMBOFILL
		s1.getmacro(getopt(itemid))
		s1.get(s1 find(s1 "#ret[]")+6)
		TO_CBFill cd.hcb s1
		
		case GRID.LVN_QG_COMBOITEMCLICK
		CB_GetItemText cd.hcb cd.cbindex s
		if(!val(s)) s1.setwintext(cd.hctrl); ret DT_Ret(hDlg 1)

#opt nowarnings 1


#sub Hook
function# nCode wParam KBDLLHOOKSTRUCT*h

int- t_edit
if nCode>=0 and t_edit and h.flags&LLKHF_UP=0
	sub.Format _s h.scanCode h.flags&LLKHF_EXTENDED 1
	_s.setwintext(t_edit)
	ret 1

ret CallNextHookEx(0 nCode wParam h)


#sub Format
function! str&s !sc ex [hook]

if(!sc) s=""; ret

if(ex) if(sc=0x36) ex=0 ;;RShift
if(sc=0x45) ;;Num Lock or Pause
	if(hook and !ex) s=""; ret ;;Pause
	s="0x45  Num Lock"; ret
 fixes Windows bugs:
    Hook gives ex for RShift.
    To remap, Num Lock must not be as ex, although it is ex.
    Pause does not have a valid scan code for remapping. Hook gives same sc as for Num Lock but no ex.

str s2.all(300)
int i=sc; if(ex) i|0xE000
s.format("0x%X  " i)

i=sc<<16; if(ex) i|0x1000000
s2.fix(GetKeyNameText(i s2 300))
if(ex and s2.len=1 and isalpha(s2[0])) s2="" ;;for some ext keys GetKeyNameText gives key name like without ext flag
s+s2

ret 1


#ret
Nothing

Special Keys
0xE05D  Application
0x000E  Backspace
0x003A  Caps Lock
0xE053  Delete
0xE04F  End
0x001C  Enter
0x0001  Escape
0xE063  Fn
0xE047  Home
0xE052  Insert
0x0038  Left Alt
0x001D  Left Ctrl
0x002A  Left Shift
0xE05B  Left Windows
0x0056  Left \ (by Z)
0x0045  Num Lock
0xE051  Page Down
0xE049  Page Up
0xE05E  Power
0xE037  PrtSc
0xE038  Right Alt
0xE01D  Right Ctrl
0x0036  Right Shift
0xE05C  Right Windows
0x0046  Scroll Lock
0xE05F  Sleep
0x0039  Space
0x000F  Tab
0xE063  Wake

QWERTY Keys
0x0028  ' "
0x000C  - _
0x0033  , <
0x0034  . >
0x0035  / ?
0x0027  ; :
0x001A  [ {
0x002B  \ |
0x001B  ] }
0x0029  ` ~
0x000D  = +
0x000B  0 )
0x0002  1 !
0x0003  2 @
0x0004  3 #
0x0005  4 $
0x0006  5 %
0x0007  6 ^
0x0008  7 &
0x0009  8 *
0x000A  9 (
0x001E  A
0x0030  B
0x002E  C
0x0020  D
0x0012  E
0x0021  F
0x0022  G
0x0023  H
0x0017  I
0x0024  J
0x0025  K
0x0026  L
0x0032  M
0x0031  N
0x0018  O
0x0019  P
0x0010  Q
0x0013  R
0x001F  S
0x0014  T
0x0016  U
0x002F  V
0x0011  W
0x002D  X
0x0015  Y
0x002C  Z

Number Pad Keys
0x0052  Num 0
0x004F  Num 1
0x0050  Num 2
0x0051  Num 3
0x004B  Num 4
0x004C  Num 5
0x004D  Num 6
0x0047  Num 7
0x0048  Num 8
0x0049  Num 9
0x004A  Num -
0x0037  Num *
0x0053  Num .
0xE035  Num /
0x004E  Num +
0xE01C  Num Enter

Arrow Keys
0xE050  Down
0xE04B  Left
0xE04D  Right
0xE048  Up

Function Keys
0x003B  F1
0x003C  F2
0x003D  F3
0x003E  F4
0x003F  F5
0x0040  F6
0x0041  F7
0x0042  F8
0x0043  F9
0x0044  F10
0x0057  F11
0x0058  F12
0x0064  F13
0x0065  F14
0x0066  F15

F-Lock Keys
0xE040  Close
0xE042  Fwd
0xE03B  Help
0xE03E  New
0xE03C  Office Home
0xE03F  Open
0xE058  Print
0xE007  Redo
0xE041  Reply
0xE057  Save
0xE043  Send
0xE023  Spell
0xE03D  Task Pane
0xE008  Undo

Media Keys
0xE020  Mute
0xE019  Next Track
0xE022  Play/Pause
0xE010  Prev Track
0xE024  Stop
0xE02E  Volume Down
0xE030  Volume Up

Web Keys
0xE06A  Back
0xE066  Favorites
0xE069  Forward
0xE032  Home
0xE067  Refresh
0xE065  Search
0xE068  Stop

Application Keys
0xE021  Calculator
0xE06C  Mail
0xE06D  Media
0xE011  Messenger
0xE06B  My Computer

Far East Keyboard
0xE070  DBE_KATAKANA
0xE077  DBE_SBCSCHAR
0xE079  CONVERT
0xE07B  NONCONVERT

Microsoft Natural Multimedia Keyboard
0xE064  My Pictures
0xE03C  My Music
0xE020  Mute
0xE022  Play/Pause
0xE024  Stop
0xE030  + (Volume up)
0xE02E  - (Volume down)
0xE010  |<< (Previous)
0xE019  >>| (Next)
0xE06D  Media
0xE06C  Mail
0xE032  Web/Home
0xE005  Messenger
0xE021  Calculator
0xE016  Log Off
0xE05F  Sleep
0xE03B  Help (on F1 key)
0xE008  Undo (on F2 key)
0xE007  Redo (on F3 key)
0xE042  Fwd (on F8 key)
0xE043  Send (on F9 key)
0xE023  Spell (on F10 key)
0xE057  Save (on F11 key)
0xE058  Print (on F12 key)

Cannot Remap
Pause
