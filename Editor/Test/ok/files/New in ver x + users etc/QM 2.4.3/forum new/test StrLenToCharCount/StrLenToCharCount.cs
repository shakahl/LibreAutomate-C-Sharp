 /
function# $s nBytes

 Converts byte count to character count in string.
 Returns the number of characters that corresponds to nBytes.

 s - string.
 nBytes - can be string length or some offset in string, in bytes.
   If -1, this function calls len() to get string length.

 REMARKS
 In Unicode mode, non-ASCII characters consist of more than 1 byte.
 len() and other QM string functions always use byte count.


opt noerrorshere 1
if(nBytes<0) nBytes=len(s)
if(!_unicode) ret nBytes
ret MultiByteToWideChar(_unicode 0 s nBytes 0 0)
