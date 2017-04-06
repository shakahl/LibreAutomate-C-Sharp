str file1="$desktop$\test1.txt"
str file2="$desktop$\test2.txt"
str file3="$desktop$\test3.txt" ;;destination file. The macro creates it.

 ------------------

str sf1.getfile(file1) sf2.getfile(file2)
ARRAY(str) a1(sf1) a2(sf2)
int i1 i2

sf1=""
for i2 0 a2.len ;;for each line in sf2
	 is this line in sf1?
	str& s=a2[i2]
	int found=0
	for i1 0 a1.len ;;for each line in sf1
		if(matchw(a1[i1] s 1)) found=1; break
	 if not, add to sf1
	if(!found) sf1.addline(s)

 out sf1

sf1.setfile(file3)
