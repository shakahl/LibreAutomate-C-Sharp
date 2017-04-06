 /
function $ext $verb $cmd [flags] ;;flags: 1 make default

 Adds a verb (context menu command) to a file type.
 Error if fails.

 ext - filename extension, eg "vig". Also can be "*" to add verb to all file types.
 verb - verb name, eg "Play".
 cmd - command line, eg "c:\program files\x\x.exe /p ''%1''".

 REMARKS
 QM must be running as administrator.

 Added in: QM 2.3.2.


int h=HKEY_CLASSES_ROOT
str s cls

if(!sub_sys.FileType_GetClass(ext cls)) end F"{ERR_FAILED}, file type not registered"

if(flags&1) rset verb "" s.from(cls "\Shell") h

s.from(cls "\Shell\" verb "\Command")
if(!rset(cmd "" s h iif(cmd[0]='%' REG_EXPAND_SZ REG_SZ))) end ERR_FAILED
