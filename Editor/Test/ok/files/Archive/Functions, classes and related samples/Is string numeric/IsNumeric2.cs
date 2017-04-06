 /
function! $s

 Returns 1 if s is a number. Returns 0 if not. Returns 0 if s is a number followed by text.
 The function interprets numbers more like Excel or Visual Basic than QM.
 Before the number can be spaces and - or +. After can be spaces.
 The number can be decimal (like 567), VB hexadecimal (like &H567) or double (like 1.45 or 5E8).
 The number can contain thousand separators (like 1,000,000).
 Decimal point and thousand separator character depends on user's locale (like in Excel).

 EXAMPLE
 str s="10"
 if IsNumeric2(s)
	 out "Numeric"
 else
	 out "Text"


double k
ret !VarR8FromStr(@s LOCALE_USER_DEFAULT 0 &k)
