 create array
str lines=
 line0
 line1
 line2
 line3
 line4
 line5
 ;;etc
 line95
 line96
 line97
 line98
 line99
 line100
ARRAY(str) arr=lines

 randomize array
int randomization_depth=2
rep arr.len*randomization_depth
	 swap two random elements
	 g1
	int i1=RandomInt(0 arr.len-1)
	int i2=RandomInt(0 arr.len-1)
	 int i1=Uniform(0 arr.len-1)
	 int i2=Uniform(0 arr.len-1)
	if(i1=i2) goto g1
	str tmp=arr[i1]
	arr[i1]=arr[i2]
	arr[i2]=tmp

 results
out
lines=arr
out lines
	