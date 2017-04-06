out
str s="164```````````````123```"
ARRAY(str) arr
RegexSplitString s "`" arr
int i
for i 0 arr.len
	out F"[{arr[i]}]"
