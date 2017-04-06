function# str&s nBytes [flags] ;;flags: 1 from the beginning, 2 whole.

 Reads from stream into a str variable.

 s - variable that receives the data.
 nBytes - number of bytes to read.
 flags:
   1 - before reading call SetPos(0). Without flags 1 or 2 reads from current position.
   2 (QM 2.4.2) - read whole stream, from the beginning. nBytes can be 0, it is ignored.

 REMARKS
 Moves stream position.
 QM 2.4.2. Returns the number of bytes read. If calling this function in loop, break the loop when returns 0, it means that reached the end of stream.


int R
if(flags&2) long n=GetSize; if(n<1000000000) nBytes=n; else end ERR_MEMORY
if(flags&3) SetPos(0)
s.all(nBytes 2)
is.Read(s nBytes &R)
if(R<nBytes) s.fix(R)
ret R
err+ end _error
