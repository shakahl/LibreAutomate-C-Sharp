function# flags int&inetflags

if(inetflags=1) inetflags=INTERNET_FLAG_KEEP_CONNECTION ;;fbc
sel flags&3
	case 0
	if(!IntIsOnline) lasterror="offline"; ret
	inetflags|INTERNET_FLAG_RELOAD|INTERNET_FLAG_NO_CACHE_WRITE
	case 1 inetflags|INTERNET_FLAG_RESYNCHRONIZE
	case 3 inetflags|INTERNET_FLAG_RELOAD|INTERNET_FLAG_NO_CACHE_WRITE

ret 1
