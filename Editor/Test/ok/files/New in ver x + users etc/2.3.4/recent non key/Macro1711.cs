DateTime x.FromComputerTime
x.AddYears(1)
x=x+1111
 out _s.timeformat("{DD} {TT}" x.t)
 out _s.timeformat("{DD} {TT}" x)
 out _s.timeformat("{DD} {TT}" &x)
 out _s.timeformat("{DD} {TT}" &x.t)
 out _s.timeformat("{DD} {TT}" x)

out x.ToStrFormat("{DD} {TT}.{F} {FF}")
