function$ _flags [nBitsNotFlags]

 Converts _flags to string like 1|2|128|0x300.
 Returns this.
 nBitsNotFlags - number of low-order bits that are not flags. Will format the bits as single non-hex number.


if(!_flags) s=0; ret s
s.all
if(nBitsNotFlags) _i=1<<nBitsNotFlags-1; if(_flags&_i) s=_flags&_i; s+"|"; _flags~_i
int i; for(i nBitsNotFlags 8) if(_flags&(1<<i)) s+(1<<i); s+"|"
_flags~255
if(_flags) s+F"0x{_flags}"; else s.rtrim('|')
ret s
