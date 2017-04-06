 /
function int'sw

deb 100
str si.getmacro(getopt(itemid) 1)
if ideb; out "%s : %s" Now(0) si
int i 
str sfnam spath

i=Store_Clipboard_File_Path(spath sfnam)
if i
	_s="Job aborted in Store_Clipboard_File_Path"
	out "<>%s : <open ''%s /%i''>%s</open> - %s" Now(0) si _error.place si _s
	mac "Warning_QM"
	deb-
	ret


 mes spath
 out spath
 out sfnam
GetEmlDataOEFile(spath)

str stag
stag=EmlSenderDate(sw)

if sw=8
 	stag.findreplace("[]" " - ")
	stag.setclip
	mes stag "Eml data stored in clip : " "i"
	deb-
	ret

str s1 s2
File_Extension(sfnam s1 si)
_s.getfilename(sfnam)
s2.from(_s "_" stag "." s1)
s2.fix
sfnam=s2
i=sfnam.ReplaceInvalidFilenameCharacters_S

ren* spath sfnam
_s.from("Eml file tag renamed to : " sfnam)
bee 300 300
Task_Message _s 0 0x8000FF

deb-
