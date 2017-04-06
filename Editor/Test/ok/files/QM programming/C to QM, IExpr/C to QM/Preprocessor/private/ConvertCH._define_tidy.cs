function str&s

 removes unnecessary spaces and some other characters to avoid duplicate definitions when differs only number of spaces.
 to be called from _define().
 incomplete, untrusted, currently not used.

if(findc(s 34)<0)
	s.replacerx("[\(] +" "(")
	s.replacerx("[\)] +" ")")
	s.replacerx(" +[\(]" "(")
	s.replacerx(" +[\(]" ")")
	s.replacerx("  +" " ")
	s.replacerx("(?<=\d)L\b" "")
