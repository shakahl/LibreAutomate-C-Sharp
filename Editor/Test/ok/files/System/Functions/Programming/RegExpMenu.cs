 \Dialog_Editor
function# [hEdit] [flags] ;;flags: 1 show the replacement menu, 2 show replacements as a submenu, 4 add Code submenu, 8 don't add 'Show errors'

 Shows menu for inserting regular expressions.
 Returns 1 if a string selected (QM 2.3.4), 0 if not.

 hEdit - handle of an Edit control where to add selected string. If omitted or 0, pastes in active window.
 flags - added in QM 2.4.3.


int R idd; str s

MenuPopup m
sub.CreateMenu(m hEdit flags)
idd=m.Show(hEdit)
sel idd
	case 0 ret
	case 2 ;;Edit
	sub.Edit(hEdit)
	
	case 3 ;;Add
	s.getwintext(hEdit); err
	s.ltrim("$"); s.findreplace("[13]" "\r"); s.findreplace("[10]" "\n")
	if(s.len) sub.UserRegexps(0 _s); _s.addline(s); sub.UserRegexps(1 _s)
	
	case 10000 ;;Help
	sub.Help
	
	case 10001 ;;Show errors
	findrx "" s.getwintext(hEdit) 0 128 _s
	err sub_sys.MsgBox hEdit _error.description "" "i"
	
	case else ;;insert the selected regexp in hEdit or in the focused control
	sub.SetEditText hEdit m idd flags
	R=1

err+
ret R


#sub SetEditText
function hEdit m idd flags

str s; int i j

if flags&0x10000 ;;window triggers
	s.getwintext(hEdit)
	if !s.beg("$")
		SendMessage(hEdit EM_GETSEL &i &j)
		SendMessage(hEdit EM_SETSEL 0 0); SendMessage(hEdit WM_CHAR '$' 0)
		SendMessage(hEdit EM_SETSEL i+1 j+1)

if(!MenuGetString(m idd &s)) ret

i=findc(s 9)+1; if(i>=s.len) ret
s.get(s i)
j=find(s iif(_unicode "[0xC2][0xA0]" "[0xA0]"))
if(j>=0) s.remove(j iif(_unicode 2 1)); j=s.len-j

if hEdit
	EditReplaceSel hEdit 0 s
	if(j>0) SendMessage(hEdit EM_GETSEL 0 &i); i-j; SendMessage(hEdit EM_SETSEL i i)
	SetFocus hEdit
else
	opt keysync 1
	if(j>0) key (s) L(#j); else key (s)

err+


#sub CreateMenu
function MenuPopup&m hEdit flags

 Normal:  label=regexp
 No label:  regexp  or  =regexp
 Disabled:  label= or .1 label
 Separator:  empty line or - or |
 To set caret position, insert (|) in regexp.

str s sa
int idFrom=1

 add submenu of user regexps
if flags&3!=1
	if(hEdit) sa.getwintext(hEdit); err
	s.from(">My[]&Edit...[]" iif(sa.len "" ".1") "&Add[]-[]")
	sub.UserRegexps(0 sa) if(sa.len) s.addline(sa)
	sa=
	 <
	 >Often used
	 0 or more any char., non-greedy=.*?
	 1 or more any char., non-greedy=.+?
	 Any char. except new line=[^\r\n]
	 <
	 >Options
	 Case insensitive=(?i)
	 Case sensitive=(?-i)
	 . matches newlines too=(?s)
	 Multiline (^ and $ for lines)=(?m)
	 Extended=(?x)
	 Invert greediness=(?U)
	 <
	;
	s+sa

 add replacements
if flags&3
	sa=
	 Whole match=$0
	 Submatch 1=$1
	 Submatch 2=$2
	 Submatch n=${(|)}
	 Part before match=$`
	 Part after match=$'
	 $=$$
	if(flags&2=0) s=sa; idFrom=1000; goto g1
	s.from(s ">Replacements[]" sa "[]<[]")

 add code samples
if flags&4
	sa=
	 >Code
	 Find=str s="text"; int i=findrx(s "regex"); if(i<0) out "not found"
	 Find and get=str s="text"; str ss; int i=findrx(s "regex" 0 0 ss); if(i>=0) out ss
	 Find, get substrings=str s="text"; ARRAY(str) a; if(findrx(s "(\w)e(.+)" 0 0 a)>=0) out a
	 Find all=str s="text"; ARRAY(str) a; if(findrx(s "." 0 4 a)) for(_i 0 a.len) out a[0 _i]
	 -
	 Replace=str s="text"; int n=s.replacerx("\w" "r"); if(n) out s
	 Replace single=str s="text"; if(s.replacerx("\w" "r" 4)>=0) out s
	 Replace, append=str s="text"; int n=s.replacerx("\w" "$0+"); if(n) out s
	 Replace, swap subsrings=str s="text"; int n=s.replacerx("(\w)(\w)" "$2$1"); if(n) out s
	 <
	;
	s+sa

 add default regexps
sa=
 -
 Any char. except newline=.
 Character in char. set=[(|)]
 Char. not in char. set=[^(|)]
 Char. in range=[(|)-]
 Char. not in range=[^(|)-]
 Digit=\d
 Nondigit=\D
 Whitespace char.=\s
 Non-whitespace char.=\S
 Word character=\w
 Non-word char.=\W
 New line=\r\n
 Tab=\t
 Literal \^$*+?.()[]{}|-=\
 Ordinary text=\Q(|)\E
 |
 0 or 1=?
 0 or more=*
 1 or more=+
 n={(|)}
 n to m={(|),}
 Non-greedy (after *+})=?
 -
 Beginning, or line beg.=^
 End, or line end=$
 Word boundary=\b
 Not word boundary=\B
 Preceded by=(?<=(|))
 Not preceded by=(?<!(|))
 Followed by=(?=(|))
 Not followed by=(?!(|))
 -
 Subexpression=((|))
 Or=|
s+sa

 g1
s.replacerx("(?m)^(.*?)=$" ".1$1")
s.replacerx("(?m)^(.+?)=" "$1[9]")
s.replacerx("(?m)^=(.+?)" " [9]$1")
s.findreplace("(|)" iif(_unicode "[0xC2][0xA0]" "[0xA0]"))
s.findreplace("[][]" "[]-[]")

m.AddItems(s idFrom 4)
if flags&3!=1
	m.AddItems("-[]&Help" 9999)
	if(hEdit and flags&8=0) m.AddItems("&Show errors" 10001)


#sub UserRegexps
function save str&s
lpstr k="Software\GinDi\QM2\Settings"
if(save) rset s "find menu" k
else rget s "find menu" k; s.rtrim("[]")


#sub Edit
function hEdit

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 208 202 "Edit my regexp submenu"
 1 Button 0x54010000 0x4 4 184 50 14 "OK"
 2 Button 0x54010000 0x4 60 184 50 14 "Cancel"
 7 Button 0x54010000 0x4 152 184 50 14 "Help"
 3 Edit 0x542110C4 0x204 4 4 200 125 ""
 4 QM_DlgInfo 0x54000000 0x20004 4 132 200 45 "Normal:  label=regexp[]No label:  regexp  or  =regexp[]Disabled:  label= or .1 label[]Separator:  empty line or - or |[]To set caret position, insert (|) in regexp."
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3"
str e3
sub.UserRegexps(0 e3)
if(!e3.end("[]")) e3+"[]"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret
sub.UserRegexps(1 e3)


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7 sub.Help
ret 1


#sub Help
#if !EXE
QmHelp "IDH_PCRE"
#else
run "http://www.quickmacros.com/help/RegExp/IDH_PCRE.html"
#endif
