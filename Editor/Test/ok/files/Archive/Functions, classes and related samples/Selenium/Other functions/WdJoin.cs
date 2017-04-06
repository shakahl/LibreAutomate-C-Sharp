function~ $sep ARRAY(BSTR)arr

 Returns arr converted to string where sep is used to join elements.
 Used instead of C# String.Join.

str s
int i
for i 0 arr.len
	if(i) s+sep
	s+arr[i]
ret s
