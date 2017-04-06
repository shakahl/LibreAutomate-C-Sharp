 out _s.expandpath("$Cookies$")
 run "$Cookies$"
 run "$Cookies$\Low"
 run "$Cache$"
 out _s.getfile("$Cache$\cookie:g@quickmacros.com/")
 out _s.getfile("$Cookies$\Low\index.dat")

 _i=10000
 if(!InternetGetCookie("http://quickmacros.com/" 0 _s.all(_i) &_i))
	 end _s.dllerror
	  end _s.dllerror("" "wininet")
 out _s.fix(_i)


 InternetGetCookie does not get cookies even for current process, or I don't know how to use it.
