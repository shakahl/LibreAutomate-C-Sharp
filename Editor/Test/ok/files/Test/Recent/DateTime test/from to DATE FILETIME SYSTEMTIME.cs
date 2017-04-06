out
#compile "__DateTime"
DateTime x y x1 x2 x3

x.FromComputerTime
 x.FromParts(15000 1 1);; error in x.ToDATE
out x.ToStr(8)

DATE d=x.ToDATE
 d="1000.1.1" ;;error in x1.FromDATE(d)
out _s.timeformat("{DD} {TT}" d)
FILETIME ft=x.ToFILETIME
out _s.timeformat("{DD} {TT}" ft)
SYSTEMTIME st=x.ToSYSTEMTIME
out _s.timeformat("{DD} {TT}" st)

x1.FromDATE(d)
x2.FromFILETIME(ft)
x3.FromSYSTEMTIME(st)
out x1.ToStr(8)
out x2.ToStr(8)
out x3.ToStr(8)
