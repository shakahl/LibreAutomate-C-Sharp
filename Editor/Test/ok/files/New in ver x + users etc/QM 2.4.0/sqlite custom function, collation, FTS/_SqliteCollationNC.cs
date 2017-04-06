 /
function[c]# param na $a nb $b
out "%i %s  %i %s" na a nb b
ret StrCompareN(a b iif(na<nb na nb) 1)
