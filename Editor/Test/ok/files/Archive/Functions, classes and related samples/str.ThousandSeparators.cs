 Inserts comma at every 3 digits in a number.

 EXAMPLE
 str s="-3456789.1234"
 s.ThousandSeparators
 out s ;;-3,456,789.1234


if(!this.len) ret
strrev this
REPLACERX r.ifrom=findc(this '.')+1
r.repl="$0,"
this.replacerx("\d{3}(?=\d)" r)
strrev this
