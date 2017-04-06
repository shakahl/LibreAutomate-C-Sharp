function double'd precision

 Converts double (floating point) value to string without extra 0 or decimal point at the end.

 precision - max number of digits after decimal point.

 EXAMPLE
 double d=14.94454592
 str s.FormatDouble(d 2)
 out s


format(_s.from("%." precision "f") d)
if(precision) rtrim("0"); rtrim(".")
