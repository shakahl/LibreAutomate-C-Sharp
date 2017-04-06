 /GFD test
function# VsHelp.DExploreAppObj&dex $fname str&dname

int retry
str sfn=fname
 g1
if(!retry) dex.DisplayTopicFromKeyword(sfn); err ret ;;finds better, but must be filtered
else dex.DisplayTopicFromF1Keyword(sfn); err ret

Htm el=htm("BODY" "" "" "+wndclass_desked_gsk" 0 0 0x20 10)
str s ss rx
s=el.Text
rx.format("(?s)\b%s\s*\((.+?)\);" fname)
int found=findrx(s rx)>=0
if(!found)
	 ret
	if(retry) ret
	retry=1
	goto g1

if(findrx(s "(?:Import ?)?library ?(\w+)\.lib\b" 1 0 ss 1)<0) ret
 out s; ret

dname.from(fname " " ss)
ret 1
