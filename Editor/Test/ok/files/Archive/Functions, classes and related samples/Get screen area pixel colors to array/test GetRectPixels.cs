out
int hwnd=id(137 "Calculator") ;;button 7
RECT r; ARRAY(int) a
GetWindowRect hwnd &r
if(!GetRectPixels(r a 0)) end "failed"
int row col
for row 0 a.len(2)
	out "row %i" row
	for col 0 a.len(1)
		out "0x%X" a[col row]
