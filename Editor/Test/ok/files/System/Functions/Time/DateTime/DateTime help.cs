 A variable of class DateTime represents date and time.
 Class DateTime is similar to DATE. Has more functions and better precision (supports milliseconds and microseconds).

 Internally date/time is stored as long, in FILETIME format.
 It is number of ticks since January 1, 1601. A tick is 0.1 microseconds.

 A DateTime variable stores absolute time. It doesn't store a time span (difference between two times).
 A time span can be stored in a variable of type long, in FILETIME format (number of 0.1 microsecond intervals). Can be negative.

 Also you can use operators with DateTime variables.
 Below are examples. Assume x and y are variables of DateTime type, and ts is variable of type long.
   x=y ;;set x = y
   ts=x-y ;;set ts = time span between x and y
   x=x+ts ;;add time span to x
   if(x<y) ... ;;if x is less than y

 Also there are several global (non member) functions to work with date/time. TimeSpanFromStr, DaysInMonth, etc.
 To show list of these functions, type "DateTime." or "time.".

 Most functions throw error if passed or calculated date/time values are out of range.
 Valid ranges of date/time parts are:
   year - 1601 to 9999. Actually may be stored dates up to about 30000, but some functions don't support it.
   month - 1 to 12.
   day - 1 to 28, 29, 30 or 31.
   hour - 0 to 23.
   minute - 0 to 59.
   second - 0 to 59.
   millisecond - 0 to 999.
   microsecond - 0 to 999.9.
   day of week - 0 (Sunday) to 6 (Saturday).
   day of year - 1 to 365 or 366.

 Added in: QM 2.3.2.

 EXAMPLE
DateTime x
x.FromComputerTime
out x.ToStr(8)
