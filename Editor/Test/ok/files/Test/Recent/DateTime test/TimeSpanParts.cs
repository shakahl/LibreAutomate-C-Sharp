out
DateTime x

long k=TimeSpanFromParts(1 2 3 4 5 999.9)

 int d h m s ms; double mcs
 TimeSpanGetParts k d h m s ms mcs
 out "%i %i %i %i %i %g" d h m s ms mcs

 long d h m s ms mcs
 TimeSpanGetPartsTotal k d h m s ms mcs
 out k
 out "%I64i %I64i %I64i %I64i %I64i %I64i" d h m s ms mcs

out TimeSpanToStr(k 4)
