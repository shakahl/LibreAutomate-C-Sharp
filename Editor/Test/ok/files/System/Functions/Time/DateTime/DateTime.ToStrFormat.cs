function'str $frm [locale] [dateFlags] [timeFlags]

 Creates custom date/time string.

 All parameters are as with <help>str.timeformat</help>.

 Some frm fields:
    {D} - short date, like "08/09/2009".
    {DD} - long date, like "August 9, 2009".
    {T} - time without seconds, like "15:59" or "3:59 PM".
    {TT} - time with seconds, like "15:59:30" or "3:59:30 PM".
 This function also supports milliseconds and ticks:
    {F} - milliseconds as 3 digits.
    {FF} - ticks as 7 digits. A tick is 0.1 microseconds.


str f=frm
if find(f "{F")>=0
	_i=t/10000%1000; _s.format("%03i" _i)
	f.findreplace("{F}" _s)
	_i=t%10000000; _s.format("%07i" _i)
	f.findreplace("{FF}" _s)

ret _s.timeformat(f this locale dateFlags timeFlags)
