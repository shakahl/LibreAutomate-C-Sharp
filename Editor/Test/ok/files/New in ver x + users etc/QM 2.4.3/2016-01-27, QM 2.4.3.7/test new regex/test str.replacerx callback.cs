out
str s rx
s="aa 7word bb 8kk cc"

rx="(\d)(\w+)"

REPLACERX r.frepl=&sub.Callback_str_replacerx
out s.replacerx(rx r)
out s


#sub Callback_str_replacerx
function# REPLACERXCB&x

 Callback function for str.replacerx.
 Called each time when a match is found during replacement process. Provides replacement string.
 x.match initially contains the matched substring. This function can modify it. It will become replacement string.
 More info in QM Help.

 Return:
 0 - match is replacement string. It will be appended to string being formatted. If match is not modified, matched substring will not be replaced.
 > 0 - nothing should be appended to string being formatted. This return value can be used either to remove matched substring, or when callback function itself appends replacement to strnew.
 -1 - stop replacement process. str.replacerx will return immediately. It returns number of replacements not including current, or -1 for single replacement mode. To stop replacement process and include current replacement, set x.rr.ito = 0.
 < -100 generate error with this error number.


out x.match
x.match.ucase

 x.strnew.addline(x.match)
 ret 1

 ret -1
 if(x.number=2) ret -1
 ret -100
 x.rrx.ito=0
