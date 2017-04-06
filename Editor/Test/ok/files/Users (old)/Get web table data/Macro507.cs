ClearOutput

ARRAY(str) an
Acc a=acc("Order #" "TEXT" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040 "parent next6 first")
a.WebTableToArray(an)
 display results
out "------ results ------"
int i
for(i 0 an.len) out an[i]
out "------ results with row and column numbers, assuming there are 7 columns ------"
int r c nc=7
for r 0 an.len/nc
	out "--- row %i ---" r+1
	for c 0 nc
		out "column %i: %s" c+1 an[r*nc+c]
		