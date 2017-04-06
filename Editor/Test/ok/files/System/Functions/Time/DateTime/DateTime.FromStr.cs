function $datetimeStr

 Initializes this variable from date/time string.
 Error if incorrect format or out of range.

 REMARKS
 Date range is 1601.1.1 - 9999.12.31.
 Date and time format can be of current locale or US.
 The string can contain date and time separated by space, like "01\01\2000 01:02:03", or only date.
 If the string contains only time, results are undefined. DateTime variables cannot contain only time. Instead use global function TimeSpanFromStr.
 Milliseconds can be specified as dot and 3 digits like "01\01\2000 01:02:03.500".
 Full precision (number of 0.1 microsecond intervals) can be specified as dot and 7 digits like "01\01\2000 01:02:03.1234567".


if(empty(datetimeStr)) t=0; ret

int ms i msfp
i=findrx(datetimeStr "\.(\d{3}|\d{7})$")
if(i>=0)
	ms=val(datetimeStr+i+1 0 msfp)
	datetimeStr=_s.left(datetimeStr i)

DATE d=datetimeStr
d.tofiletime(+&t)

sel msfp
	case 3 t+ms*10000
	case 7 t+ms

err+ end ERR_BADARG
