 /Selenium help
function# $by $value ;;by: "class name", "css selector", "id", "name", "link text", "partial link text", "tag name", "xpath"


str sd sr
sd.format("{%s,%s}" _JsonPairStr("using" by 1) _JsonPairStr("value" value 1))
_Post("element" sd sr)
if(!_JsonGetValue(sr "ELEMENT" _s)) end sr
ret val(_s)
