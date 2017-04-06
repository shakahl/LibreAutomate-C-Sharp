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


str& s=x.match
if(findcs(s "`~")>=0) mes "Note: contains `~"
 s.findreplace("Format(" "Form(" 4)
s.remove(4 2)
s.findreplace("$" "%s")
s.findreplace("#" "%i")
s.findreplace("&" "%X")
s.findreplace("`" "%c")
s.findreplace("~" "%s")
