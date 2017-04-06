 This macro creates ini file containing sundays and saturdays of next 10 years and of the remainder of this year.
 Run this macro. Then edit the file: add other holidays and possibly remove sun/saturdays that aren't holidays. For each day, use format daynumber=anything or just daynumber=.
 The file later will be used by IsTodayHoliday.


str inifile.expandpath("$my qm$\nbd.ini")
if(dir(inifile))
	mes- "The file already exists. Overwrite?" "" "OC!"
	mes- "Are you sure?" "" "OC!"
int nyears=10 ;;you can change this; it is number of years not including this year

str ini
DATE d.getclock
SYSTEMTIME st
int y ymax month
rep
	d.tosystemtime(st)
	
	if(st.wYear!=y)
		y=st.wYear
		if(!ymax) ymax=y+nyears
		if(y>ymax) break
	
	if(st.wMonth!=month) month=st.wMonth; ini.formata("[%i-%i][]" y month) ;;write year and month as ini section
	
	sel st.wDayOfWeek
		case [0,6] ini.formata("%i=%s[]" st.wDay iif(st.wDayOfWeek "sat" "sun")) ;;write sundays and saturdays
	
	 other (or some of other) holidays can be added/removed here instead of editing the ini file. Example:
	sel st.wMonth
		case 1 sel(st.wDay) case 1 ini.formata("%i=%s[]" st.wDay "New Year")
		 and so on
	
	d=d+1

 out; out ini
ini.setfile(inifile)
inifile-"''"; inifile+"''"
run "notepad.exe" inifile ;;open for editing
