; Input: ARRAY A > List of extensions
;         s      > extension to search
; Returns: index of A, that matches s. If not found, returns the last index.


function# ARRAY(str)'&A ~s
if(!s.len)s="."
for _i 0 A.len
	if (matchw(s A[_i] 1))
		ret _i
ret (A.len-1)
