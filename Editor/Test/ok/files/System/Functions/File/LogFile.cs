 /
function ~s [flags] [$logfile] ;;flags: 1 date/time

 Writes to a log file.

 s - string or number to write.
 logfile - file name. Default: <help>_logfile</help> (special global variable).
    QM 2.3.5. Creates parent folder if does not exist.

 REMARKS
 If file size exceeds <help>_logfilesize</help> (special global variable, default is 1 MB), removes first half (oldest data).

 EXAMPLES
 LogFile "string"

 int a=10
 LogFile a 1


if(empty(logfile)) logfile=_logfile
File f.Open(logfile "a+")

int i=f.FileLen
if(!_logfilesize) _logfilesize=1024*1024
if(i>_logfilesize)
	f.SetPos(_logfilesize/2)
	f.ReadToStr(_s)
	f.Close
	f.Open(logfile "w")
	i=findc(_s 10)+1
	f.Write(_s+i _s.len-i)

if(flags&1) s-_s.timeformat("{D} {TT}[]")
f.WriteLine(s)

err+ end ERR_FAILED
