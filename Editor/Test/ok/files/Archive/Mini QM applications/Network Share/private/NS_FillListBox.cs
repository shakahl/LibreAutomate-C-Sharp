 /
function hlb

SendMessage hlb LB_RESETCONTENT 0 0
ARRAY(str) a; int i
RegGetValues a "\NetShare"
for i 0 a.len
	LB_Add hlb a[i]
