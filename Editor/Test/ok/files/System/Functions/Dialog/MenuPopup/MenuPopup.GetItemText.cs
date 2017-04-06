function! item [str&s] [str&sRight]

 Gets menu item text.
 Returns: 1 success, 0 failed.

 item - item id. If <=0, item index.
 s - receives text. If sRight used, receives only the left-aligned part. Can be 0.
 sRight - receives the right-aligned part of text. Can be 0.


str ss
if(&s) s.all; else &s=ss
if(&sRight) sRight.all

if(!MenuGetString(m_h item &s)) ret

if &sRight
	int i=findc(s 9)
	if(i>=0)
		sRight=s+i+1
		s.fix(i)
ret 1
