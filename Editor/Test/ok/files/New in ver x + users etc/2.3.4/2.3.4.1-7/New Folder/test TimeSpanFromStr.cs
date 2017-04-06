out
Q &q
long t
rep 1000
	 t=TimeSpanFromStr("5")
	 t=TimeSpanFromStr("1 10")
	 t=TimeSpanFromStr("1 10:05")
	 t=TimeSpanFromStr("1 10:05:25")
	 t=TimeSpanFromStr("1 10:05:25.55")
	 t=TimeSpanFromStr("10:05")
	 t=TimeSpanFromStr("10:05:25")
	 t=TimeSpanFromStr("10:05:25.55")
	 t=TimeSpanFromStr("05:25.55")
	 t=TimeSpanFromStr("25.55")
	
	 t=TimeSpanFromStr("-5")
	 t=TimeSpanFromStr("-1 10")
	 t=TimeSpanFromStr("-1 10:05")
	 t=TimeSpanFromStr("-1 10:05:25")
	 t=TimeSpanFromStr("-1 10:05:25.55")
	 t=TimeSpanFromStr("-10:05")
	 t=TimeSpanFromStr("-10:05:25")
	 t=TimeSpanFromStr("-10:05:25.55")
	 t=TimeSpanFromStr("-05:25.55")
	 t=TimeSpanFromStr("-25.55")
	
	 t=TimeSpanFromStr(" 5 ")
	 t=TimeSpanFromStr(" 1 10 ")
	 t=TimeSpanFromStr(" 1 10:05 ")
	 t=TimeSpanFromStr(" 1 10:05:25 ")
	 t=TimeSpanFromStr(" 1 10:05:25.55 ")
	 t=TimeSpanFromStr(" 10:05 ")
	 t=TimeSpanFromStr(" 10:05:25 ")
	 t=TimeSpanFromStr(" 10:05:25.55 ")
	 t=TimeSpanFromStr(" 05:25.55 ")
	 t=TimeSpanFromStr(" 25.55 ")
	
	 t=TimeSpanFromStr("5.")
	 t=TimeSpanFromStr("1 10:05:25.55E-3")
	
	 t=TimeSpanFromStr("5")
	 t=TimeSpanFromStr("1 10")
	 t=TimeSpanFromStr("1 10:05")
	 t=TimeSpanFromStr("1 10:59:59")
	 t=TimeSpanFromStr("1 10:05:25.9999998")
	 t=TimeSpanFromStr("1 10:05:25.0000001")
	 t=TimeSpanFromStr("1 10:05:25.99999999")
	 t=TimeSpanFromStr("1 10:05:25.1E1")
	 t=TimeSpanFromStr("10:05")
	 t=TimeSpanFromStr("10:05:25")
	 t=TimeSpanFromStr("10:05:25.55")
	 t=TimeSpanFromStr("05:25.55")
	 t=TimeSpanFromStr("25.55")
	
	 err out _error.description
Q &qq
outq
out TimeSpanToStr(t 4)
