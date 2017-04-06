 /
function!

 Returns 1 if today is a holiday (Sunday, Saturday, etc), 0 if not.
 Uses "$my qm$\nbd.ini" that is created using macro "Create Holidays File". Error if the file does not exist.

 EXAMPLE
 if IsTodayHoliday
 	out "holiday"


str inifile="$my qm$\nbd.ini"
if(!dir(inifile)) mes- "File not found:[]%s[][]Read macro 'Create Holidays File'." "" "x" inifile

DATE d.getclock
SYSTEMTIME st
d.tosystemtime(st)
str s1 s2(st.wDay) s3.format("%i-%i" st.wYear st.wMonth)
ret rget(s1 s2 s3 inifile)
