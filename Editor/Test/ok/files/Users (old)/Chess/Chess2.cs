ARRAY(POINT) a
GetChessboardArray a 1

 display what is in the array
int i j
for i 0 8
	for j 0 8
		out "%c%i: x=%i y=%i" 'A'+i j+1 a[i j].x a[i j].y

 ...
