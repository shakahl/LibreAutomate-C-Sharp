 /
function# [year] [month]

 Returns number of days in month.

 year - year. If 0, uses this year.
 month - month. If 0, uses this month. Error if invalid.

 Added in: QM 2.3.2.


if(!year or !month)
	DateTime d.FromComputerTime
	int y m
	d.GetParts(y m)
	if(!year) year=y
	if(!month) month=m

sel month
	case [1,3,5,7,8,10,12] ret 31
	case [4,6,9,11] ret 30
	case 2 ret 28+(year%4=0 and (year%100 or year%400=0)) ;;thanks Ulf
end ERR_BADARG
