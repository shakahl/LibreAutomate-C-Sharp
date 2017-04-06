
_s=F"A {_s.from(1 2)} C"
_s=F"{_s.from(1 2)}"
_s=F"{_i} {5.5} {'A'} {_s.from(1 2)} C"
_s=F"{_s.from(`str` 2)}"
_s=F"{_i%%05i} {_s} C"
_s=F"{_s.from(`str` 2)}"
_s=F"{{{_s.from(1 2)}"


out (win("str" "" 1)+_s[0+1])
