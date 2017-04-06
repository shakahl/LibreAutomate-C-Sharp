function# str&line

 Reads one line.
 Returns: 1 success, 0 EOF.

 line - variable that receives the line.

 REMARKS
 Reads starting from current file pointer, and updates it.


if(!m_file) end ERR_INIT

int n1 n2
if(line.nc<250) line.all(250); else line.len=0
rep
	n1=line.nc-line.len
	if(fgets(line+line.len n1 m_file))
		n2=len(line+line.len)
		line.fix(line.len+n2)
		if(n2<n1-1 or line[line.len-1]=10) line.rtrim("[]"); ret 1
		line.all(line.len+1000 1)
	else if(line.len) ret 1
	else ret
