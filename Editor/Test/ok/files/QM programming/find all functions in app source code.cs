out
str sf="$qm$\runstrfunc_r.cpp"
str st.getfile(sf)

ARRAY(str) af
ARRAY(CHARRANGE) ac
findrx st "^.+?\b(\w+)\(.+[]\{[]((?s).+?)[]\}" 0 12 af
int i n
for i 0 af.len
	 out af[2 i]
	str& s=af[2 i]
	n=findrx(s "\bF;" 0 4 ac)
	out "%s  %i" af[1 i] n
	 out "-----"
	