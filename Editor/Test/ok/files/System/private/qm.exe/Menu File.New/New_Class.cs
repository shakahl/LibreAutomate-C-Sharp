 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 200 195 "New Class"
 3 Static 0x54000000 0x0 4 6 42 12 "Class name"
 4 Edit 0x54030080 0x200 48 4 128 14 "cn"
 11 Button 0x54032000 0x0 180 4 16 14 "?"
 15 Static 0x54000000 0x0 4 24 42 18 "Member variables"
 16 Edit 0x54231044 0x200 48 22 128 32 "var"
 12 Button 0x54032000 0x0 180 22 16 14 "?"
 5 Static 0x54000000 0x0 4 60 42 18 "Member functions"
 6 Edit 0x54231044 0x200 48 58 128 71 "f"
 13 Button 0x54032000 0x0 180 58 16 14 "?"
 7 Button 0x54012003 0x0 4 132 54 13 "Constructor"
 8 Button 0x54012003 0x0 66 132 50 13 "Destructor"
 9 Button 0x54012003 0x0 124 132 50 13 "Operator ="
 17 Button 0x54032000 0x0 180 130 16 14 "?"
 18 Static 0x54000000 0x0 4 150 42 12 "Base class"
 19 Edit 0x54030080 0x200 48 148 128 14 "bas"
 20 Button 0x54032000 0x0 180 148 16 14 "?"
 1 Button 0x54030001 0x4 4 177 48 14 "OK"
 2 Button 0x54030000 0x4 56 177 48 14 "Cancel"
 10 Button 0x54032000 0x0 180 177 16 14 "?"
 14 Static 0x54000010 0x20000 0 170 208 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

str controls = "4 16 6 7 8 9 19"
str e4cn e16var e6f c7Con c8Des c9Ope e19bas
c8Des=1
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret

str s s2 sdef

e16var.trim
if(numlines(e16var)>1) e16var.replacerx("(?m)^" "[9]"); e16var-"[]"
else if(e16var.len) e16var-" "

if(e19bas.len) e19bas-" :"

sdef.format("class %s%s%s[]" e4cn e19bas e16var)

int idfolder=newitem(e4cn "" "Folder")
foreach s e6f
	if(!s.len) continue
	s.from(e4cn "." s)
	newitem s "" "Member" "" idfolder 16

if(c9Ope=1) newitem s.from(e4cn "=") s2.format("function %s&source[][] Operator =[]" e4cn) "Member" "" idfolder
if(c8Des=1) newitem s.from(e4cn ".") " Destructor[]" "Member" "" idfolder
if(c7Con=1) newitem e4cn " Constructor[]" "Member" "" idfolder
newitem s2.format("__%s" e4cn) sdef "" "" idfolder

s.format(" This macro will be displayed when you click this class name in code editor and press F1.[] Add class help here. Add code examples below EXAMPLES.[][] EXAMPLES[][]#compile ''__%s''[]%s x[]" e4cn e4cn)
newitem s2.from(e4cn " help") s "" "" idfolder 4

out "<>Compile __%s to let QM know about the class. To compile, you can use this code: <code>#compile ''__%s''</code>. You can add this code anywhere where you use the class, or in function <open>init2</open>." e4cn e4cn

int i=qmitem("init2" 1)
if(!i) newitem "init2" " Function init2 runs when QM starts or this QM file [re]opened.[] It can contain declarations, initialize global variables, start other threads (mac)...[]" "Function"

err+ out _error.description


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_COMMAND goto messages2
ret
 messages2
__strt vn; str s s2
sel wParam
	case 11 sub.TT hDlg wParam "Examples:[][]TestClass[]__HiddenClass"
	case 12 sub.TT hDlg wParam "Example:[][]x y width height[]-m_hmenu[]--ARRAY(int)m_a[]str'name str'__m_hidden[][]Members with - (hyphen) will be protected. They can be used only in functions of this and inherited classes.[]Members with -- will be private. They can be used only in functions of this class."
	case 13 sub.TT hDlg wParam "Example:[][]Function[]__HiddenFunction[][]Later you can use function's Properties dialog to make it protected or private.[]To hide a function, you also can move it into a private folder."
	case 17 sub.TT hDlg wParam "Special user-defined functions called by QM.[][]Constructor runs when a variable of this class is created.[]Destructor runs before destroying it.[]Operator= runs when assigning another variable of this class to it."
	case 20 sub.TT hDlg wParam "A class from which the new class will inherit member functions and variables. Optional.[]Examples:[][]MyBaseClass[]-MyProtectedBaseClass[]--MyPrivateBaseClass[]MyBaseClass'_b"
	case 10 sub.TT hDlg wParam "This dialog adds initial class definition and functions. Later you can add/remove/edit everything in QM."
	case IDOK
	vn.getwintext(id(4 hDlg)); if(!vn.IsValidName) mes "Invalid class name."; ret
	for(_i 0 3)
		sel(_i) case 1 s2="."; case 2 s2="="
		if(qmitem(_s.from(vn s2) 1)) mes F"A QM item '{_s}' already exists."; ret
	
	s.getwintext(id(19 hDlg))
	if(s.len and findrx(s "^-{0,2}[a-z_][0-9a-z_]*('[a-z_][0-9a-z_]*)?$" 0 1)) mes "Invalid base class name."; ret
	
	s2.getwintext(id(6 hDlg))
	foreach vn.s s2
		if(vn.len and !vn.IsValidName) mes F"Invalid function name {vn}"; ret
ret 1


#sub TT
function hDlg cid $s
OsdHide "New_Class"
sub_sys.TooltipOsd s 8 "New_Class" -1 0 0 id(cid hDlg)