 /
function $ext $cls $description $icon [$cmd] [$content_type]

 Adds new file type.
 Error if fails.

 ext - filename extension.
 cls - class. Can be any string but must not begin with ".".
 description - eg "Vig File".
 icon - eg "c:\program files\x\x.exe,3".
 cmd - command line, eg "c:\program files\x\x.exe ''%1''". Will be registered as "Open" verb. If you don't use it here, then later you should use FileTypeAddVerb.
 content_type - content type, eg "text/xml". Optional.

 REMARKS
 QM must be running as administrator.

 Added in: QM 2.3.2.

 EXAMPLE
 FileTypeRegister "vig" "vigfile" "Vig File" "shell32.dll,20" "notepad.exe ''%1''"
 
 str s="test"
 s.setfile("$desktop$\test.vig")
 run "$desktop$\test.vig"


int h=HKEY_CLASSES_ROOT
str s

if(ext[0]='.') ext+1
if(!rset(cls "" s.from("." ext) h)) end ERR_FAILED

if(!empty(content_type)) rset content_type "Content Type" s h

if(!rset(description "" cls h)) end ERR_FAILED

if(!empty(icon)) rset icon "" s.from(cls "\DefaultIcon") h iif(icon[0]='%' REG_EXPAND_SZ REG_SZ)

if(!empty(cmd)) FileTypeAddVerb ext "Open" cmd 1; err end _error

SHChangeNotify SHCNE_ASSOCCHANGED 0 0 0
