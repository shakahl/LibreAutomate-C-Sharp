 /
function ARRAY(POINT)&a arrayId [recreate]

 Records chessboard square coordinates to an array. Saves the array to a file. If the array is already created previously, retrieves it from the file instead.
 Use arrayId to identify different arrays. It can be any numeric value. It is appended to the filename.
 If recreate is used and nonzero, creates the array again, instead of getting from file.


a.create(8 8)

str s filename
int al=64*sizeof(POINT)

filename.from("$my qm$\chess" arrayId ".arr")
if(!recreate) s.getfile(filename); err

if(s.len==al)
	memcpy(&a[0 0] s al)
else
	int i j
	for i 0 8
		for j 0 8
			mes- "Click %c%i" "" "OC" 'A'+i j+1 ;;message box
			wait 0 ML ;;wait for mouse click
			xm a[i j] ;;get the coordinates into the array
	
	s.all(al 2)
	memcpy(s &a[0 0] al)
	s.setfile(filename)
