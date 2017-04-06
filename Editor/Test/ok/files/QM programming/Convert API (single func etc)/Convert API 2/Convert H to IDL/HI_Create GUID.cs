GUID g
str s
olelib.CoCreateGuid(+&g)
s.fix(olelib.StringFromGUID2(+&g +s.all(100) 100)*2)
out s.ansi
