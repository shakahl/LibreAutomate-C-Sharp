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
if(arr.len<3) ret

 randomize array
int randomization_depth=2
rep randomization_depth
	int i
	for i 0 arr.len
		 swap with a random element
		 g1
		int i2=Uniform(0 arr.len-1)
		if(i2=i) goto g1
		str tmp=arr[i]
		arr[i]=arr[i2]
		arr[i2]=tmp

 results
out
lines=arr
out lines
	