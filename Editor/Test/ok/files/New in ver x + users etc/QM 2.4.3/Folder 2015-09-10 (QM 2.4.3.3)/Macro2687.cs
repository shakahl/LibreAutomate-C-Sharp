out
opt end 1
str s="one 3hh op 5mm"

 PF
 int i=s.replacerx("\b\d(\w+)" "A$1B")
  PN;PO
 out i

REPLACERX r
 r.repl="A$1B"
 r.ifrom=5
 r.ifrom=3
 r.ito=6
r.frepl=&sub.Cb
out s.replacerx("\b\d(\w+)" r)
err out _s.getstruct(_error 1); ret

out s


#sub Cb
function# REPLACERXCB&x

 Return:
 0 - match is replacement string. It will be appended to string being formatted. If match is not modified, matched substring will not be replaced.
 > 0 - nothing should be appended to string being formatted. This return value can be used either to remove matched substring, or when callback function itself appends replacement to strnew.
 -1 - stop replacement process. str.replacerx will return immediately. It returns number of replacements not including current, or -1 for single replacement mode. To stop replacement process and include current replacement, set x.rr.ito = 0.
 < -100 generate error with this error number.


out x.match
x.match+";"
x.match.ucase

 ret E_INVALIDARG
