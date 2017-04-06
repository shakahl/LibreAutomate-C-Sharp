typelib VBScript_RegExp_55 {3F4DACA7-160D-11D2-A8E9-00104B365C9F} 5.5

str s="Word1 word2 word3 word22 word[]word1"
str repl="replaced"

VBScript_RegExp_55.RegExp re._create
re.Global=TRUE
re.IgnoreCase=TRUE
re.Pattern="word[1-2]\b"
 VARIANT v=repl
 out re.Replace(s &v)
VBScript_RegExp_55.MatchCollection collection=re.Execute(s)
foreach str'ss collection
	out ss
	