/Dialog_Editor
str controls = "3"
str lb3

lb3="&Project (.hhp)[]&Index (.hhk)[]&Alias (.ali)[]&Map (.h)[]&Contents (.hhc)"

if(!ShowDialog("HelpAddTopic" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 79 "Help - add topic"
 3 ListBox 0x54230109 0x204 4 4 96 48 ""
 1 Button 0x54030001 0x4 4 60 48 14 "OK"
 2 Button 0x54030000 0x4 54 60 48 14 "Cancel"
 4 Static 0x54020000 0x4 122 6 100 70 "Please open new topic in Dreamweaver. This will add that topic to qm2help.[][]Please check that topic title is correct (in Dreamweaver), and topic file name is correct, and file is in correct folder."
 END DIALOG
 DIALOG EDITOR: "" 0x2010500 "*" ""

int hDW=win("Dreamweaver" "_macr_dreamweaver_frame_window_")
int hedit=id(59893 hDW)
str s.getwintext(hedit)
s.get(s 8); s.findreplace("/" "\"); s[1]=':'

str sFile sTitle sDef
int i
ARRAY(str) a

if(findrx(s "app\\HTMLHelp\\(.+\\(\w+)\.html?)$" 3 1 a)<0) ret
sFile=a[1]
sDef=a[2]
 out sFile
 out sDef

int htitle w1=win("Document" "Afx:*" "" 0x800)
if(w1) htitle=id(27008 w1) ;;floating tb
else htitle=child(27008 "" "Edit" hDW) ;;docked tb
sTitle.getwintext(htitle)
 out sTitle
 ret

mes- "File: %s[]Def: %s[]Title: %s" "" "OC" sFile sDef sTitle

int hwnd=win("HTML Help Workshop"); if(hwnd) clo hwnd; wait 0 WD hwnd

if(lb3[0]='1') ;;add to project
	s.getfile("$qm$\htmlhelp\qm2help.hhp")
	s.replacerx("(?<=[]\[FILES\][])" _s.from(sFile "[]") 4)
	s.setfile("$qm$\htmlhelp\qm2help.hhp")

if(lb3[1]='1') ;;add to index
	s.getfile("$qm$\htmlhelp\qm2help.hhk")
	_s.format("[9]<LI> <OBJECT type=''text/sitemap''>[][9][9]<param name=''Name'' value=''%s''>[][9][9]<param name=''Name'' value=''%s''>[][9][9]<param name=''Local'' value=''%s''>[][9][9]</OBJECT>[]" sTitle sTitle sFile)
	s.replacerx("(?<=[]<UL>[])" _s 4)
	s.setfile("$qm$\htmlhelp\qm2help.hhk")

if(lb3[2]='1') ;;add to alias
	s.getfile("$qm$\htmlhelp\qm2help.ali")
	s.formata("%s=%s[]" sDef sFile)
	s.setfile("$qm$\htmlhelp\qm2help.ali")

if(lb3[3]='1') ;;add to map (help.h)
	s.getfile("$qm$\help.h")
	i=findrx(s "\d+$")
	if(i>=0) i=val(s+i)+1; else inp- i "Could not determine help context id."
	s.formata("#define %s[9][9]%i[]" sDef i)
	s.setfile("$qm$\help.h")

if(lb3[4]='1') ;;add to contents
	run "$program files$\HTML Help Workshop\hhw.exe" "" "" "" 0xA00 "HTML Help Workshop" hwnd
	SelectTab(child(1088 "" "SysTabControl32" hwnd) 1)
	mes "Where to insert new topic? In contents, please select item that will be above new item." "" "?"
	but id(360 hwnd)
	sTitle.setwintext(child(1001 "" "Edit" "Table of Contents Entry" 0x5))
	but child("&Add..." "Button" "Table of Contents Entry" 0x1)
	sFile.setwintext(id(1132 "Path or URL"))
	but id(1 "Path or URL")
	but id(1 "Table of Contents Entry")
