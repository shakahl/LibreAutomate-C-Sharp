 Sorting is about 20 times faster when you use C callback function instead of QM callback function. And you don't need another QM function.

str s.RandomString(10000 10000)

str c=
 int qsort_c(char* a, char* b)
 {
 if(*a<*b) return -1;
 if(*a>*b) return 1;
 return 0;
 }
__Tcc x.Compile(c "qsort_c")

Q &q
qsort s s.len 1 x.f
Q &qq
outq
out s
