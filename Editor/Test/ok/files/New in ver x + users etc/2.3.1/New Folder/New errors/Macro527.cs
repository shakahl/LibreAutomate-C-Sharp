_i/0
 end "hhh"
 act "hhhhhhhhhhh"
 end "eeeee"
 end "40eeeee"

 memcpy +7 &_i 1
 RaiseException EXCEPTION_ACCESS_VIOLATION 0 0 0
 err out _error.code
err
	out _error.code
	out _error.description
	out _error.iid
	out _error.line
	out _error.place
	out _error.source
