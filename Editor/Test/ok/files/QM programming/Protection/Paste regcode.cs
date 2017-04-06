 \Dialog_Editor
 Asks for name. Generates and inserts regcode.

str s ss name
int i iri ver lic
str controls = "4 5 6 10 12 14"
str e4nam c5Inc cb6Lic cb10Ver e12n e14ord

e4nam.getclip; e4nam.trim
 e12n=1
cb10Ver="2.0.x[]2.1.x[]&2.2.x (current)"
 cb6Lic="&Normal[]Temporary[]Pro"
cb6Lic="&Normal[]Temporary 1 week[]Temporary 2 weeks[]Temporary 3 weeks[]Temporary 4 weeks"
 c5Inc=1

if(!ShowDialog("Paste regcode" 0 &controls)) ret
name=e4nam
ver=val(cb10Ver)
lic=val(cb6Lic)
iri=c5Inc=1

int n=val(e12n)
if(!n and !empty(e14ord)) n=1
if(n) name.formata(", %i license%s" n iif(n=1 "" "s"))
if(!empty(e14ord)) name.formata(", ordered by %s" e14ord)

if(iri)
	s.getmacro("reg info")
	i=find(s "[][][]")+4
	ss.get(s i)
	s[i]=0
	if(!win or wintest(win "" "" "explorer")) act
	outp s

if(ver=0)
	if(lic) mes- "invalid license for old"
	Generate_regcode_old(name)
else
	s=Generate_regcode(name lic ver lic)
	outp s
	0.1

if(iri) outp "%s[]" ss

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 234 176 "Generate regcode"
 1 Button 0x54030001 0x4 6 158 48 15 "OK"
 2 Button 0x54030000 0x4 58 158 48 15 "Cancel"
 3 Static 0x54000000 0x0 10 18 48 13 "Name"
 4 Edit 0x54030080 0x200 60 17 164 14 "nam"
 5 Button 0x54012003 0x0 10 130 62 13 "Include info"
 6 ComboBox 0x54230243 0x0 60 111 96 213 "Lic"
 7 Static 0x54000000 0x0 10 111 48 13 "License"
 9 Static 0x54000000 0x0 10 94 48 12 "Version"
 10 ComboBox 0x54230243 0x0 60 94 96 213 "Ver"
 11 Static 0x54000000 0x0 10 37 48 12 "n licenses"
 12 Edit 0x54032000 0x200 60 34 32 15 "n"
 13 Static 0x54000000 0x0 10 55 48 12 "Ordered by"
 14 Edit 0x54030080 0x200 60 53 126 14 "ord"
 16 Static 0x54000000 0x0 192 54 34 12 "(optional)"
 17 Static 0x54000000 0x0 98 36 36 13 "(optional)"
 15 Button 0x54020007 0x0 4 4 226 70 "Customer"
 8 Button 0x54020007 0x0 4 82 226 66 "License type"
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "*" "" ""
