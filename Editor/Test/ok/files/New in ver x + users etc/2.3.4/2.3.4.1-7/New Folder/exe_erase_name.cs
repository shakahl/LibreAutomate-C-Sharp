 /
function str&s $name $repl [flags] ;;1 erase WORD before

int i n
rep
	i=findb(s name -1 i)
	if(i<0) break
	memcpy s+i repl len(repl)
	 s[i]='_'
	if(flags&1) s[i-1]=0; s[i-2]=0
	n+1

out n
if(!n) end "not found"
