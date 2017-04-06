function $s [lens]

 Sends data.
 Error if failed.

 s - string or binary data to send.
 lens - number of bytes to send. If omitted or <0, uses s string length.

 REMARKS
 This function does not wait until server receives data.


if(getopt(nargs)<2 or lens<0) lens=len(s)

rep
	_i=sock_send(m_socket s lens 0)
	if(_i<0) E
	if(_i>=lens) break
	lens-_i; s+_i
