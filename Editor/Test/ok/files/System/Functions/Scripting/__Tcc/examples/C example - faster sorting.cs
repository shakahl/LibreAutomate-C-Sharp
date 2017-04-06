 Sorting is much faster when you use C callback function instead of QM callback function.

str s.RandomString(10000 10000)

str c=
 int qsort_c(void* param, char* a, char* b)
 {
 if(*a<*b) return -1;
 if(*a>*b) return 1;
 return 0;
 }
__Tcc x.Compile(c "qsort_c")

int t1=perf

q_sort s s.len 1 x.f 0

int t2=perf
out t2-t1

out s.fix(2000)
