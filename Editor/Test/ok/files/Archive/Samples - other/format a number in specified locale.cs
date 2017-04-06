double d=1.23
str s
setlocale LC_NUMERIC "Lithuanian"
s.fix(_snprintf(s.all(1000) 1000 "%g" d))
out s

double dd=atof(s)
out dd

 Here "Lithuanian" is a locale where , is decimal separator
 by default (but can be changed in Control Panel -> Regional...).
 Read more in MSDN Library.
 Note that setlocale changes locale for whole process (all threads).
 Intrinsic QM functions are not affected, unless you use QM 2.2.0 (then don't use setlocale).
