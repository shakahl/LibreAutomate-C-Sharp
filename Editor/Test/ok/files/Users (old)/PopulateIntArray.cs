 /
function ARRAY(int)&a str's [dim1]

 Converts string containing integer values to array of integers.
 If dim1 is omitted or 0, creates array of single-dimension. If dim1
 is greater than 0, creates array of two dimensions, where length of
 first dimension is dim1.

 EXAMPLES
 int i j
 ARRAY(int) a
 
 out "-- Single dimension --"
 PopulateIntArray a "5 6 7"
 for(i 0 a.len) out a[i]
 
 out "-- Two dimensions; length of first dimension is 3 --"
 PopulateIntArray a "5 7, 2 12, 1 4" 3 ;;commas here are used only to make the code easier to read
 for(i 0 a.len(1))
	 for(j 0 a.len(2))
		 out "a[%i %i] = %i" i j a[i j]


ARRAY(lpstr) la
tok s la -1 "" 1
if(!la.len) end ERR_BADARG

int i j k dim2
if(dim1>0)
	dim2=la.len/dim1; if(!dim2) end ERR_BADARG
	a.create(dim1 dim2)
	for i 0 dim1
		for j 0 dim2
			a[i j]=val(la[k])
			k+1
else
	a.create(la.len)
	for i 0 la.len
		a[i]=val(la[i])
