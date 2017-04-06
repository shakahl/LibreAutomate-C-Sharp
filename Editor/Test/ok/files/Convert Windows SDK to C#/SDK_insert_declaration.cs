 \
function str'name str'decl

 Inserts decl in file csFile, before line //insert new declaration here
 You can specify this function name in dialog "Windows API for C#" (function SDK_fine_declaration shows it) field "Run macro".
 If the file is open in Visual Studio, it nicely updates editor text, even with Undo, if the file is saved. Tested with Visual Studio 2015.

str csFile="Q:\app\Catkeys\Test Projects\Test\TestProg.cs" ;;change this


 out name
 out decl

str s.getfile(csFile)
int i=findrx(s "[][ \t]*//insert new declaration here")
if(i<0) end "the file must have line ''//insert new declaration here''"
decl-"[]"
decl.replacerx("(?m)^" "[9]") ;;tab-indent
s.insert(decl i)
s.setfile(csFile 0 -1 0x100)
