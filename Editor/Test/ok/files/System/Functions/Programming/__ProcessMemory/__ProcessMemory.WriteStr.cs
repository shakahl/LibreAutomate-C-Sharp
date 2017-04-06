function $s [offset] [flags] ;;flags: 1 convert to UTF-16

 Writes string to the memory allocated by Alloc.
 Error if fails.

 s - a string.
 offset - offset in the allocated memory.
 flags:
    1 - convert to Unicode UTF-16 format.


int i
if(flags&1) s=_s.unicode(s); i=_s.len+2
else i=len(s)+1

Write(s i offset)
err end _error
