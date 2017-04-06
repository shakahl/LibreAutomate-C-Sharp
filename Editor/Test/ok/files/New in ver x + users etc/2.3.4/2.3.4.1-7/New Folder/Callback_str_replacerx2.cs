 /
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


 out x.lenv ;;3. Whole match and 2 submatches.
 x.vec[1].cpMin
 
  x.match.fromn
 x.match.format("%.*s ~ %


str s1.get(x.subject x.vec[1].cpMin (x.vec[1].cpMax-x.vec[1].cpMin))
str s2.get(x.subject x.vec[2].cpMin (x.vec[2].cpMax-x.vec[2].cpMin))

x.match=F"{s1.lcase} ~ {s2.ucase}"
