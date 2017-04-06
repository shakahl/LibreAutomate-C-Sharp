function str&s nBytes [offset] [flags] ;;flags: 1 convert from UTF-16

 Reads from the memory allocated by Alloc to a str variable.
 Error if fails.

 s - str variable.
 nBytes - number of bytes to read.
 offset - offset in the allocated memory.
 flags:
    1 - convert from Unicode UTF-16 format to normal format. nBytes must be number of bytes in the UTF-16 string (number of characters * 2).


s.all(nBytes 2)
Read(s nBytes offset)
err end _error
if(flags&1) s.ansi
