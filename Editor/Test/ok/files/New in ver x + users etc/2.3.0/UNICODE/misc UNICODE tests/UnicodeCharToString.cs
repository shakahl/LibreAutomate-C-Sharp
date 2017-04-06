 /
function~ ch

 Converts Unicode character to string.
 In Unicode mode the string will be UTF-8.
 Otherwise it will be ANSI, and Unicode characters that cannot be translated to ANSI will be converted to ?.


if(ch&0xffff0000) end ERR_BADARG

str s
lpstr ss=+&ch
s.ansi(ss)

ret s
