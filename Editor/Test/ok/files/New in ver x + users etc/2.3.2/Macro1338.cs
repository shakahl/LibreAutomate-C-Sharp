ARRAY(int) a; int i
win "name" "class" "" 0 0 0 a ;;get handles of all matching windows
for i 0 a.len ;;for each window in the array
	if hid(a[i]) ;;if hidden
		hid- a[i]
		