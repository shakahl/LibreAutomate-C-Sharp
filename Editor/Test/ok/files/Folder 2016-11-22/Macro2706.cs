out
str path="c:\.txt"

int n=1000
str icon.all(n)

int hr=AssocQueryString(0 ASSOCSTR_DEFAULTICON path 0 icon &n)
if(hr) _s.dllerror(path "" hr); out F"<><c 0xff>{_s}</c>"; ret
icon.fix(n)
out F"{icon}"
