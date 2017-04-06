str s="one <u>two</u> three <u>four</u>"
IRegex x=CreateRegex("<u>(.+?)</u>")
x.ReplaceCallback(s &sub.Callback)
out s


#sub Callback
function# cbParam str&match REGEXREPLACECB&x

x.x.GetStr(1 match)
match.ucase
