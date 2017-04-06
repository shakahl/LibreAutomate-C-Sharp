 /
function# $_file ARRAY(str)&a

 Parses csv file and creates array of two dimensions.
 First dimension - column, second - row.
 Returns number of columns. Number of rows is a.len.

 A csv file is text file that contains table where
 rows are stored in separate lines, and columns are
 separated by semicolons. Supported by Excel.

 This function does not support semicolons and quotes
 within cells.

 EXAMPLE
 ARRAY(str) a
 int nc=CsvFileToArray("$desktop$\id.csv" a)
 int i j
 for i 0 a.len
	 out "-----Row %i-----" i+1
	 for j 0 nc
		 out a[j i]


str s ss.getfile(_file) sss
int i j k li nc(1) nl(numlines(ss))
if(!nl) a.redim; ret

foreach s ss
	if(!a.len)
		 find number of columns
		for(j 0 9999999) j=findc(s ';' j); if(j>=0) nc+1; else break
		a.create(nc nl)
	
	j=0; k=0
	for(i 0 nc)
		j=findc(s ';' k)
		if(j!k) a[i li].get(s k j-k)
		if(j<0) break
		k=j+1
	
	li+1

ret nc
err+ end _error
