 /
function! $s [flags] ;;flags: 1 full

 Returns 1 if s begins with a number. Returns 0 if not.
 Before the number can be spaces and - or +.
 The number can be decimal (like 567), hexadecimal (like 0x567) or double (like 1.45 or 5E8).

 s - string.
 flags:
   1 - return 0 if the string begins with a number but contains text after it.

 EXAMPLE
 str s="10"
 if IsNumeric(s)
	 out "Numeric"
 else
	 out "Text"


val s 2 _i
if(!_i) ret
if(flags&1) if(s[_i]) ret
ret 1
