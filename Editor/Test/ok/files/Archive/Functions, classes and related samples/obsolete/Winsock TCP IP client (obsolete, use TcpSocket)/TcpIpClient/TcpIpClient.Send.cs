function! $s [lens] [flags] ;;flags: 1 send all

 Sends data.
 Returns 1. If failed, returns 0.

 s - string or binary data to send.
 lens - number of bytes to send. If omitted or <0, uses s string length.

 REMARKS
 May fail if data is too big. Use flag 1 to send in parts, until all sent.


if(getopt(nargs)<2 or lens<0) lens=len(s)

rep
	_i=send(m_socket s lens 0)
	if(_i<0) ret
	if(_i>=lens) ret 1
	if(flags&1=0) ret
	lens-_i; s+_i
