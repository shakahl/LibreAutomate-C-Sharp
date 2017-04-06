 out
ARRAY(int) a
SAFEARRAY** p

p=&a; out p ;;bug. Gets a.psa instead.
p=&a.psa; out p ;;correct

byte* b=&a; out b
b=&a.psa; out b

_i=&a; ;;out _i
_i=&a.psa; ;;out _i


 Htm e
 MSHTML.IHTMLElement* k
 k=&e; out k
 k=&e.el; out k

 type PTR1 SAFEARRAY*m
 PTR1 u
 p=&u; out p
 p=&u.m; out p

BSTR t
word** tt
tt=&t; out tt ;;bug
tt=&t.pstr; out tt

 str s
 byte** ss
 ss=&s; out ss
 ss=&s.lpstr; out ss

 ARRAY(int) a.create(1)
 SAFEARRAY* p=a
 out a.psa
 out p

