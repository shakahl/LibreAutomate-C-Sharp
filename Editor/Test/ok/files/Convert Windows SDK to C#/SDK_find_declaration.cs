 Finds a Windows API declaration in Api.cs.
 How to use:
   In your C# editor type a Windows API name, move the mouse over it and run this macro.
   Or move the mouse over a Windows API name anywhere, for example in API documentation.
   To run this macro you can use a hotkey trigger; default is Ctrl+D.
   The macro shows a dialog with the API name, its C# declaration (empty if not found) and several options.
   In the dialog review the declaration, maybe change options, and click OK.
   If you start this macro while it is already running, it replaces text in the dialog.
   Also you can edit text in the dialog Name field to search for a new name.
 How it works:
   Double-clicks to select word from mouse, and gets the word through the clipboard.
   Finds the declaration in Api.cs file and shows it in dialog together with options.
   Api.cs file full path must be specified in the dialog.
   The macro can copy the declaration to the clipboard or/and run a macro that for example can insert the declaration in a C# file. Example - function SDK_insert_declaration.


spe 20
int w=win(mouse)
if w!=_hwndqm or !WinTest(child(mouse) "Toolbar*") ;;prevent double clicking the Run button
	if(w!=win) act w; key LR ;;prevent VS selecting whole line when there is selected text
	dou
	str name.getsel
	name.trim

if getopt(nthreads)>1
	w=win("Windows API for C#" "#32770")
	if w
		name.setwintext(id(4 w))
		act w
		ret

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 552 166 "Windows API for C#"
 3 Static 0x54000000 0x0 8 8 28 12 "Name"
 4 Edit 0x54030080 0x200 40 8 152 12 "Nam" "Windows API name (a function, struct, enum, delegate, const, interface, COM class, Guid, PROPERTYKEY).[]In your C# editor type the name, move the mouse over it and run this macro."
 6 Edit 0x54231044 0x200 8 24 536 72 "Decl"
 10 Static 0x54000000 0x0 16 116 38 13 "File Api.cs"
 11 Edit 0x54030080 0x200 56 116 208 13 "Api"
 7 Button 0x54012003 0x0 288 116 76 13 "Copy to clipboard"
 14 Button 0x54012003 0x0 376 116 54 13 "Run macro"
 15 Edit 0x54030080 0x200 432 116 104 13 "Mac" "Should begin with:[]function str'name str'decl"
 1 Button 0x54030001 0x4 8 144 48 14 "OK"
 2 Button 0x54030000 0x4 60 144 48 14 "Cancel"
 5 Button 0x54020007 0x0 8 100 536 37 "Options"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "4 6 11 7 14 15"
str e4Nam e6Dec e11Api c7Cop c14Run e15Mac
e4Nam=name
rget e11Api "Api.cs" "\CsharpWinAPI" 0 "$my qm$\Api.cs"
rget c7Cop "bClipboard" "\CsharpWinAPI" 0 "1"
rget c14Run "bMacro" "\CsharpWinAPI"
rget e15Mac "sMacro" "\CsharpWinAPI"

if(!ShowDialog(dd &sub.DlgProc &controls)) ret

rset e11Api "Api.cs" "\CsharpWinAPI"
rset c7Cop "bClipboard" "\CsharpWinAPI"
rset c14Run "bMacro" "\CsharpWinAPI"
rset e15Mac "sMacro" "\CsharpWinAPI"

if(!e6Dec.len) ret

if c7Cop=1
	e6Dec.setclip

if c14Run=1 and e15Mac.len
	mac e15Mac "" e4Nam e6Dec

err+ mes _error.description "" "x"


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	sub.Find hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	
	case EN_CHANGE<<16|4
	sub.Find hDlg
	
	case EN_CHANGE<<16|11
	_s.getwintext(lParam)
	if(FileExists(_s)) sub.Find hDlg
ret 1


#sub Find
function hDlg

str apiFile.getwintext(id(11 hDlg))
str name.getwintext(id(4 hDlg))

if(!apiFile.len) mes "Api.cs file not specified."; ret
if(!FileExists(apiFile)) mes "Api.cs file not found."; ret

type __CSHARPWINAPICACHE IStringMap'm str'path DateTime'time long'size
__CSHARPWINAPICACHE+ __g_csWinApiCache

DateTime t; long z
FileGetAttributes apiFile z t
if apiFile!=__g_csWinApiCache.path or t!=__g_csWinApiCache.time or z!=__g_csWinApiCache.size
	sub.ApiFileToStringMap __g_csWinApiCache.m apiFile
	if(__g_csWinApiCache.m.Count=0) mes "Found 0 declarations in Api.cs file."; ret
	__g_csWinApiCache.path=apiFile; __g_csWinApiCache.time=t; __g_csWinApiCache.size=z
	 out "loaded"

if(empty(name)) ret
str decl
__g_csWinApiCache.m.Get2(name decl)
str decl32
if(__g_csWinApiCache.m.Get2(F"{name}__32" decl32)) decl+"[][]"; decl+decl32
if(decl.len) decl+"[]"
decl.setwintext(id(6 hDlg))

err+ mes _error.description "" "x"


#sub ApiFileToStringMap
function IStringMap&m $apiFile

m._create; m.Flags=2

str s.getfile(apiFile)

ARRAY(str) a

str rxType="(?ms)^(?:\[[^\r\n]+\r\n)*internal (?:struct|enum|interface|class) (\w+)[^\r\n\{]+\{(?:\}$|.+?^\})"
str rxFunc="(?m)^(?:\[[^\r\n]+\r\n)*internal (?:static extern|delegate) \w+\** (\w+)\(.+;$"
str rxVarConst="(?m)^internal (?:const|readonly|static) \w+ (\w+) =.+;$"

if(!findrx(s rxType 0 4 a)) end "failed" 1
sub.AddToMap a m
if(!findrx(s rxFunc 0 4 a)) end "failed" 1
sub.AddToMap a m
if(!findrx(s rxVarConst 0 4 a)) end "failed" 1
sub.AddToMap a m


#sub AddToMap
function ARRAY(str)&a IStringMap'm

int i
for i 0 a.len
	m.Add(a[1 i] a[0 i])
