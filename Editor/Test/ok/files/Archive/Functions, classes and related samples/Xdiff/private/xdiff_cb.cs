 /
function[c]# str&so __xdiff.mmbuffer_t*a n

int i
 out n
 for(i 0 n) out "'%s'" _s.left(a[i].ptr a[i].size)

for(i 0 n) so.geta(a[i].ptr 0 a[i].size)
