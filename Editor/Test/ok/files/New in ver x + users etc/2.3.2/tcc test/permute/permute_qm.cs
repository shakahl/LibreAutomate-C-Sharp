 /
function $s str&sout [n] [l] ;;n is 0-based position of first char, l string length

 EXAMPLE
 str s
 permute("abcd" s)
 out s


if(l<1) l=len(s)

int i c

if n>=l
	 No characters to permute . . print string.
	sout.formata("%s[]" s)
else
	 Work through all characters of string.
	for i n l
		 Swap this character with the first one.
		c = s[i]; s[i] = s[n]; s[n] = c
		 Permute remainder of string.
		permute_qm(s sout n+1 l)
		 Swap back again.
		c = s[i]; s[i] = s[n]; s[n] = c
