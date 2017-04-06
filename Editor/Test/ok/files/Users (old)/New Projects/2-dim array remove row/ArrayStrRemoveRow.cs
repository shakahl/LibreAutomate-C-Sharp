 /
function ARRAY(str)&a row

 Removes one row from 2-dim str array.
 Error if some argument is invalid.


if(a.ndim!=2 or row<a.lbound or row>a.ubound) end ERR_BADARG

str* b=&a[a.lbound(1) row]
int i inrow=a.len(1)
for(i 0 inrow) b[i].all

inrow*sizeof(str)
int nmove=a.ubound-row*inrow
if(nmove) memmove b b+inrow nmove; memset &a[a.lbound(1) a.ubound] 0 inrow

a.redim(a.len-1 a.lbound)

err+ end _error
