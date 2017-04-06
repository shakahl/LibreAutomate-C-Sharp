 \
function ?a1 [?a2] [ANY'a3] [ANY'a4] [ANY'a5] [ANY'a6] [ANY'a7] [ANY'a8] [ANY'a9] [ANY'a10] [ANY'a11] [ANY'a12] [ANY'a13] [ANY'a14] [ANY'a15] [ANY'a16] [ANY'a17] [ANY'a18] [ANY'a19]

int i
ANY* p=&a1
for i 0 getopt(nargs)
	ANY& x=p[i]
	out "0x%X %s" x.ta x.ts
	out x.ToStr
