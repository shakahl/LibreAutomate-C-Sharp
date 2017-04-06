out
#compile "__DateTime"
DateTime x y x1 x2 x3

x.FromComputerTime

 x.FromStr("2000.1.1 4:5:6.123")
 x.FromStr("2000.1.1 4:5:6.1234567")
 x.FromStr("2000.1.1")
 x.FromStr(".500")
 x.FromStr("1601.1.1")
 x.FromStr("9999.12.31")
 x.FromStr("2000/1/1")
 x.FromStr("")

out x.ToStr
out x.ToStr(8)
out x.ToStr(16)
out x.ToStr(4|8|16)
out x.ToStr(1|4|8|16)
out x.ToStr(2|4|8|16)
 out x.ToStr(1)
 out x.ToStr(2)
 out x.ToStr(2|4)
 out x.ToStr(2|4|8)
 out x.ToStr(2|8)

double mcs; x.GetParts(0 0 0 0 0 0 0 mcs); out mcs
out x.ToStrFormat("{DD} {TT}.{F}")
out x.ToStrFormat("{DD} {TT}.{FF}")
