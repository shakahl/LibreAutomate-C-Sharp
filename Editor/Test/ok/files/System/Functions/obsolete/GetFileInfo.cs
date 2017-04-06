 /
function$ [$_file] [flags] [int*attributes] [int*size] [!*timemodified] [!*timecreated] [!*timeaccessed] [timetype] ;;timetype: 0 FILETIME* or long*, 1 SYSTEMTIME*, 2 DATE*, 4 (flag) get UTC time.

 Gets file info.
 Returns file name.
 Obsolete, use FileGetAttributes.

 _file, flags - same as with <help>Dir.dir</help>.
 attributes - receives file attributes (combination of FILE_ATTRIBUTE... constants).
 timemodified, timecreated, timeaccessed - receive file times, in format specified by timetype.
 timetype - time format that you need. Time arguments must be addresses of variables of type:
   0 - FILETIME or long.
   1 - SYSTEMTIME.
   2 - DATE.
   Also you can add 4, which tells that the time must be UTC rather than local.
   Read more in <help #IDP_OLE_DATE>QM Help</help>.

 EXAMPLE
 SYSTEMTIME st
 int attr size
 lpstr s=GetFileInfo("winapi.txt" 0 &attr &size &st 0 0 1)
 if(s) out "%s attr=0x%X size=%u date=%u/%u/%u time=%u:%u:%u" s attr size st.wMonth st.wDay st.wYear st.wHour st.wMinute st.wSecond


Dir-- d
lpstr r=d.dir(_file flags); if(!r) ret
if(attributes) *attributes=d.fd.dwFileAttributes
if(size) *size=d.fd.nFileSizeLow
if(timemodified)
	sel(timetype&3)
		case 0 d.TimeModified(+timemodified 0 0 timetype&4)
		case 1 d.TimeModified(0 +timemodified 0 timetype&4)
		case 2 d.TimeModified(0 0 +timemodified timetype&4)
		case else end ""
if(timecreated)
	sel(timetype)
		case 0 d.TimeCreated(+timecreated 0 0 timetype&4)
		case 1 d.TimeCreated(0 +timecreated 0 timetype&4)
		case 2 d.TimeCreated(0 0 +timecreated timetype&4)
		case else end ""
if(timeaccessed)
	sel(timetype)
		case 0 d.TimeAccessed(+timeaccessed 0 0 timetype&4)
		case 1 d.TimeAccessed(0 +timeaccessed 0 timetype&4)
		case 2 d.TimeAccessed(0 0 +timeaccessed timetype&4)
ret r
