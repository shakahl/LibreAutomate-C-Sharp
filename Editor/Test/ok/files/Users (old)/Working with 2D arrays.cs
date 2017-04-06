ARRAY(int) a.create(100 100) ;;create 2D array of integers
a[0 0]=1 ;;set element
int el=a[75 99] ;;get element
a.redim(200) ;;resize the right dimension to 200 elements
a.redim(-1) ;;add 1 element to the right dimension

 display all elements
int i j
for i 0 a.len(1)
	for j 0 a.len(2)
		out a[i j]

a.createlb(100 1 100 1) ;;create 2D array where lower bound of each dimension is 1 (default is 0)
