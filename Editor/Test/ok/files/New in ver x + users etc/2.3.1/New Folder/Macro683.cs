DATE d1.getclock d2("8:00 AM") d3("5:00 PM")
_i=d1.date; d1.date=d1.date-_i ;;get time part
if(d1.date>=d2.date and d1.date<d3.date)
	out "time between 8:00 AM and 5:00 PM"
else
	out "other time"
