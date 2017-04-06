 /
function i str&sBin

 Converts from decimal (base-10) number to binary (base-2) number, eg 5 to "101".
 Although there are 32 bits, leading 0 bits are trimmed.

 EXAMPLE
 str s
 DecToBin 5 s
 out s


sBin.all(32)
itoa i sBin 2
sBin.fix
