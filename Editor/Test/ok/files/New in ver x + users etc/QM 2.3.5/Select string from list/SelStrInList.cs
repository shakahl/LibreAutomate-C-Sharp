 /
function# $sList $sFind [flags] ;;flags: 1 case insens

 Returns 1-based index of string in a list, or 0 if not found.

 sList - multiline list of single-line strings.
 sFind - string to find.


int i(1) ins(flags&1)
str s
foreach s sList
	if(!StrCompare(s sFind ins)) ret i
	i+1
