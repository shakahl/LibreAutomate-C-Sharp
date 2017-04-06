DateTime x.FromStr("2000.01.01 00:00:00")

 x.AddParts(0 24)
 x.AddParts(-1 -4)
 x.AddParts(0 0 0 0 0 123.4)

 x.AddStr("10")
 x.AddStr("-10")
 x.AddStr("5:6:7")
 x.AddStr("-5:6:7")
 x.AddStr("-10 5:6:7")
 x.AddStr("1:2:3.456")
 x.AddStr("1:2:3.4567891")
 x.AddStr("-1:2:3.4567891")
 x.AddStr("-50 1:2:3.4567891")

 x.AddYears(5)
 x.AddYears(-5)
 x.AddMonths(15)
 x.AddMonths(-15)


out x.ToStr(16)
