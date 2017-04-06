 deb
#compile "__DateTime"
DateTime x
x.FromComputerTime
 out x.ToStr(8)
 out TimeSpanToStr2(10000000)
 _s=TimeSpanToStr2(10000000)
out _s.from(TimeSpanToStr(10000000) TimeSpanToStr(20000000))
