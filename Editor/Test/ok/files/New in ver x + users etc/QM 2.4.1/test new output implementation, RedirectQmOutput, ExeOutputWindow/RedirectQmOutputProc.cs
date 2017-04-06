 /
function# str&s reserved

if &s
	out "debug TEXT"
	 out
	 s="my text"; ret
	OutputDebugString s
	 spe
	 s+" <MODIFIED>"
else
	out "debug CLEAR"
	 out
	OutputDebugString "<CLEAR>"

ret 1
