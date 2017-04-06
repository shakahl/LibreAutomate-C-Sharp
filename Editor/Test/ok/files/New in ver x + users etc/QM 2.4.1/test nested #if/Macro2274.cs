def TEST 5
#if !TEST
out "TEST is 0"
#else
rep 2
	#if TEST>=5
	out "TEST is >= 5"
	#else
	out "TEST is < 5"
	#endif
#endif
