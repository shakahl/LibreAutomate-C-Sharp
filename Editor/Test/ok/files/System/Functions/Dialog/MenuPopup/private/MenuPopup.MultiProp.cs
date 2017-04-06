function $items prop ;;prop: 0 disable, 1 check, 2 delete, 3 bold

 items - list of item ids, like "3 4 7" or "1 4-7".


int i j k
ARRAY(lpstr) a

tok items a -1 " "
for i 0 a.len
	lpstr s=a[i]
	j=val(s 0 k); if(!k) continue
	if(s[k]!'-') k=j; else k=val(s+k+1); if(k<j) continue
	for j j k+1
		sel prop
			case 0 EnableMenuItem m_h j MFS_DISABLED
			case 1 CheckMenuItem m_h j MFS_CHECKED
			case 2 DeleteMenu m_h j 0
