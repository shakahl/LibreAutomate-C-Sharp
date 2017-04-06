out
str s rx
s="aa 7word bb 8kk cc"
rx="(?<first>\d)(?<second>\w+)"
IRegex y=CreateRegex(rx)
out y.ReplaceAll(s "/${second}.${first}/")
out s
