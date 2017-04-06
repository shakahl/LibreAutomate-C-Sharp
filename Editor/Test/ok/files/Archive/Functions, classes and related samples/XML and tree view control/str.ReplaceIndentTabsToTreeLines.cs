 EXAMPLE
 out
 str s
 StringFromTreeView s id(2202 _hwndqm)
 s.ReplaceIndentTabsToTreeLines
 out s


if(!_unicode) end "must be Unicode mode"

ARRAY(lpstr) a; int i j k; lpstr t
if(tok(this a -1 "[]")<2) ret

for i 0 a.len
	t=a[i]
	for j 0 1000000000
		if(t[0]!9) break
		if(t[1]=9)
			t[0]=1 ;;│
			 replace some extra │
			if i+1=a.len or (a[i+1][j]!9 and (i+2=a.len or a[i+1]+j<a[i+2]))
				for(k i -1 -1) if(a[k][j]=1) a[k][j]=32; else a[k][j]=3; break
		else if(i+1<a.len and a[i+1][j]=9 and (i+2=a.len or a[i+1]+j<a[i+2])) t[0]=2 ;;├
		else t[0]=3 ;;└
		t+1

 at first used  characters 1, 2, 3. Now replace with multibyte UTF-8 characters.
findreplace("[1]" "│")
findreplace("[2]" "├")
findreplace("[3]" "└")
