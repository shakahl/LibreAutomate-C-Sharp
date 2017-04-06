ARRAY(POINT)+ g_chess_array
int+ g_chess_array_filled

if(!g_chess_array_filled)
	ARRAY(POINT)& a=g_chess_array
	a.create(8 8) ;;create 8x8 array
	
	int i j
	for i 0 8
		for j 0 8
			mes- "Click %c%i" "" "OC" 'A'+i j+1 ;;message box
			wait 0 ML ;;wait for mouse click
			xm a[i j] ;;get the coordinates into the array
	
	 display what is in the array
	 for i 0 8
		 for j 0 8
			 out "%c%i: x=%i y=%i" 'A'+i j+1 a[i j].x a[i j].y
	g_chess_array_filled=1

 ...
