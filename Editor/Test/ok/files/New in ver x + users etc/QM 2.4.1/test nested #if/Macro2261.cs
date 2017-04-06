out 0
#if 1
	out 1
	#if 2
		out 11
		#ifdef 1
			out 21
		#endif
		#ifndef 2
			out 22
		#endif
	#else
		out 12
	#endif
	 #e k
	 aa #endif
#else
	out 2
#endif
out 3
