int+ X=2

#if X=0
out 0
#endif

#if X=1
out 1
#endif

#if (X=0)||(X=2)
out "0 or 2"
#endif

#if (X>=2)&&(X<=4)
out "2 to 4"
#endif
