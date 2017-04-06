str ws="www.quickmacros.com"

str s
if(RunConsole2(_s.format("ping -n 3 -w 1000 %s" ws) s)) mes- "failed"
s.replacerx("[[]]+" "[]")
mes s
