out
deb 500
out "begin"
#if 1
	out "yes1"
	#if 1
		out "yes yes"
		#ifdef VT_I4
			out "yes yes yes"
		#else
			out "yes yes no"
		#endif
	#else
		out "yes no"
	#endif
	out "yes2"
#else
	out "no1"
	#if 1
		out "no yes"
	#else
		out "no no"
	#endif
	out "no2"
#endif
out "end"
