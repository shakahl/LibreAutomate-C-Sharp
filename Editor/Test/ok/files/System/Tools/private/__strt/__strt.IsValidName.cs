function!

 Returns 1 if name (this) is valid for a function, variable etc, ie does not contain invalid characters etc.
 Does not modify this. Use VN() for it.

_i=__LenId(s)
ret _i and !s[_i]
