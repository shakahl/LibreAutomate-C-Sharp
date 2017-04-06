 /
function# str&match $s $rx [subMatch] [compileFlags] [matchFlags]


opt noerrorshere
IRegex x
if(!x) x=CreateRegex(rx compileFlags)
if(!x.MatchNext(s matchFlags)) ret
x.GetStr(subMatch match)
ret 1
