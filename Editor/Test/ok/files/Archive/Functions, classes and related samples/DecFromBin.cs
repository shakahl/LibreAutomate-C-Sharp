 /
function# $sBin

 Converts from binary (base-2) number to decimal (base-10) number, eg "101" to 5.

 EXAMPLE
 int i=DecFromBin("101")
 out i


if(!sBin) ret
ret strtoul(sBin 0 2)
