 /
function'str long'timeSpan [flags] ;;return value format: "[-][days ]hours:minutes:seconds[.ms3|.ticks7]".  flags: 1 always with days, 2 with ms, 4 with ticks

 Creates string from time span.
 Returns string in format: "[-][days ]hours:minutes:seconds[.ms3|.ticks7]".

 timeSpan - time span. It is number of 0.1 microsecond intervals.

 REMARKS
 If days is 0, does not add, unless flag 1 used.
 Adds milliseconds if flag 2 used.
 Adds ticks if flag 4 used. A tick is 0.1 microseconds.

 Added in: QM 2.3.2.


int d h m s ms
str f sd; lpstr neg

if(timeSpan<0) timeSpan=-timeSpan; neg="-"
TimeSpanGetParts(timeSpan d h m s ms)

if(d or flags&1) sd.from(d " ")

f.format("%s%s%02i:%02i:%02i" neg sd h m s)

if(flags&4) _i=timeSpan%10000000; f.formata(".%07i" _i)
else if(flags&2) f.formata(".%03i" ms)

ret f
