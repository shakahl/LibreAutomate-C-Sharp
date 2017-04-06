 /Selenium help
function $by $value ARRAY(int)&a ;;by: "class name", "css selector", "id", "name", "link text", "partial link text", "tag name", "xpath"


a=0
str sd sr
sd.format("{%s,%s}" _JsonPairStr("using" by 1) _JsonPairStr("value" value 1))
_Post("elements" sd sr)
 out sr
if(!_JsonGetValue(sr "value" _s 5)) end sr
if(!_s.len) ret
 out _s
ARRAY(str) as; int i
if(!findrx(_s "''(\d+)''" 0 4 as 1)) ret
a.create(as.len)
for(i 0 a.len) a[i]=val(as[1 i])
